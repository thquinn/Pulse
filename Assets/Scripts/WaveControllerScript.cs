using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class WaveControllerScript : MonoBehaviour {
    public static WaveControllerScript instance;

    static float PREWAVE_PAUSE = 3;
    static float SHRINK = 1, MIN_DIST = 5, MAX_DIST = 12;
    static int SCORE_EMITTER = 1000;

    public GameObject prefabEmitter;

    public BorderScript borderScript;
    public PlayerScript playerScript;
    public UITextPopupContainerScript textPopupContainerTime, textPopupContainerScore;

    [HideInInspector] public int waveNumber;
    [HideInInspector] public float timeLeftInWave;
    [HideInInspector] public int score;
    int hitsThisWave;

    List<EmitterScript> emitters;
    float prewavePause;

    void Start() {
        instance = this;
        emitters = new List<EmitterScript>();
        waveNumber = 1;
        prewavePause = PREWAVE_PAUSE;
    }
    void StartWave() {
        List<Vector2> spawnCoors;
        do {
            spawnCoors = GetSpawnCoors();
        } while (spawnCoors.Any(c => Vector2.Distance(c, playerScript.transform.localPosition) < 2));
        // Spawn.
        foreach (Vector2 spawnCoor in spawnCoors) {
            GameObject emitter = Instantiate(prefabEmitter);
            emitter.transform.localPosition = spawnCoor;
            emitters.Add(emitter.GetComponent<EmitterScript>());
        }
        // Set timer.
        timeLeftInWave = GetTimeLimitForWave(waveNumber);
        hitsThisWave = 0;
    }
    List<Vector2> GetSpawnCoors() {
        List<Vector2> samples = PoissonDiskSampler.Sample(borderScript.size.x - SHRINK, borderScript.size.y - SHRINK, MIN_DIST, 100);
        samples.Shuffle();
        // Add connected coors until done.
        List<Vector2> spawnCoors = new List<Vector2>();
        spawnCoors.Add(samples[0]);
        samples.RemoveAt(0);
        int n = GetEmitterCountForWave(waveNumber);
        while (samples.Count > 0 && spawnCoors.Count < n) {
            bool found = false;
            for (int i = 0; i < samples.Count; i++) {
                foreach (Vector2 spawnCoor in spawnCoors) {
                    if (Vector2.Distance(samples[i], spawnCoor) < MAX_DIST) {
                        spawnCoors.Add(samples[i]);
                        samples.RemoveAt(i);
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            if (!found) break;
        }
        // Center all coordinates.
        Vector2 mins = new Vector2(spawnCoors.Min(c => c.x), spawnCoors.Min(c => c.y));
        Vector2 maxes = new Vector2(spawnCoors.Max(c => c.x), spawnCoors.Max(c => c.y));
        Vector2 shift = (mins + maxes) / -2;
        return spawnCoors.Select(c => c + shift).ToList();
    }
    int GetEmitterCountForWave(int waveNumber) {
        return 2 + waveNumber;
    }
    float GetTimeLimitForWave(int waveNumber) {
        return 15 + GetEmitterCountForWave(waveNumber) * 5;
    }
    public void ScoreEmitterKill() {
        score += SCORE_EMITTER * waveNumber;
    }

    void Update() {
        for (int i = emitters.Count - 1; i >= 0; i--) {
            if (emitters[i] == null) emitters.RemoveAt(i);
        }
        if (emitters.Count > 0) {
            timeLeftInWave = Mathf.Max(timeLeftInWave - Time.deltaTime, 0);
            if (emitters.Count == 1) {
                emitters[0].noPulseOnDeath = true;
            }
        } else if (prewavePause == 0) {
            EndWave();
        } else if (prewavePause > 0) {
            prewavePause = Mathf.Max(prewavePause - Time.deltaTime, 0);
            if (prewavePause == 0) {
                StartWave();
            }
        }
    }
    void EndWave() {
        Invoke("DissolveAllPulses", 1);
        prewavePause = PREWAVE_PAUSE;
        // Wave skip.
        if (timeLeftInWave >= 20) {
            waveNumber++;
            int bonus = SCORE_EMITTER * GetEmitterCountForWave(waveNumber) * waveNumber;
            bonus += Mathf.FloorToInt(GetTimeLimitForWave(waveNumber) * 1000);
            score += bonus;
            textPopupContainerScore.AddPopup($"wave skip bonus +{bonus:N0}");
        }
        // Time left bonus.
        int millisLeft = Mathf.FloorToInt(timeLeftInWave * 1000);
        score += millisLeft;
        textPopupContainerScore.AddPopup($"time bonus +{millisLeft:N0}");

        waveNumber++;
        // Grow border.
        float initialArea = borderScript.initialSize.x * borderScript.initialSize.y;
        float aspect = borderScript.initialSize.x / borderScript.initialSize.y;
        float areaMult = GetEmitterCountForWave(waveNumber) / (float)GetEmitterCountForWave(1);
        areaMult = Mathf.Pow(areaMult, 0.5f);
        float area = initialArea * areaMult;
        float height = Mathf.Sqrt(area / aspect);
        float width = height * aspect;
        borderScript.targetSize = new Vector2(width, height);
    }
    void DissolveAllPulses() {
        PulseScript[] pulses = FindObjectsByType<PulseScript>().ToArray();
        foreach (PulseScript pulse in pulses) {
            pulse.Dissolve();
        }
        if (pulses.Length > 0) {
            SFXScript.SFXPulseDissolve();
        }
    }

    public void GotHit() {
        int penalty = 2 + 2 * hitsThisWave;
        timeLeftInWave = Mathf.Max(0, timeLeftInWave - penalty);
        hitsThisWave++;
        if (hitsThisWave == 1) {
            textPopupContainerTime.AddPopup($"hit -{penalty}<voffset=.35em><size=50%>000");
        } else {
            textPopupContainerTime.AddPopup($"hit ×{hitsThisWave} -{penalty}<voffset=.35em><size=50%>000");
        }
    }
}

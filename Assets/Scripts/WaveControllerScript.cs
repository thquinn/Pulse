using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveControllerScript : MonoBehaviour {
    public static WaveControllerScript instance;

    static float PREWAVE_PAUSE = 3;
    static float SHRINK = 1, MIN_DIST = 5, MAX_DIST = 12;

    public GameObject prefabEmitter;

    public BorderScript borderScript;
    public PlayerScript playerScript;
    public UITextPopupContainerScript textPopupContainerTime, textPopupContainerScore;

    [HideInInspector] public int waveCount;
    [HideInInspector] public float timeLeftInWave;
    [HideInInspector] public int score;
    int hitsThisWave;

    List<EmitterScript> emitters;
    float prewavePause;

    void Start() {
        instance = this;
        emitters = new List<EmitterScript>();
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
        waveCount++;
        timeLeftInWave = 30;
        hitsThisWave = 0;
    }
    List<Vector2> GetSpawnCoors() {
        List<Vector2> samples = PoissonDiskSampler.Sample(borderScript.size.x - SHRINK, borderScript.size.y - SHRINK, MIN_DIST, 100);
        samples.Shuffle();
        // Add connected coors until done.
        List<Vector2> spawnCoors = new List<Vector2>();
        spawnCoors.Add(samples[0]);
        samples.RemoveAt(0);
        int n = 3;
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

    void Update() {
        if (Input.GetKeyDown(KeyCode.F2)) {
            textPopupContainerScore.AddPopup("blah " + Random.Range(10000, 9999999));
        }
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
        // Destroy all pulses.
        PulseScript[] pulses = FindObjectsByType<PulseScript>().ToArray();
        foreach (PulseScript pulse in pulses) {
            pulse.Dissolve();
        }
        prewavePause = PREWAVE_PAUSE;
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

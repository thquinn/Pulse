using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveControllerScript : MonoBehaviour {
    static float MIN_DIST = 4, MAX_DIST = 8;

    public GameObject prefabEmitter;

    public BorderScript borderScript;

    void Start() {
        List<Vector2> samples = PoissonDiskSampler.Sample(borderScript.size.x, borderScript.size.y, MIN_DIST, 100);
        samples = samples.Select(s => s - borderScript.size / 2).ToList().Shuffle();
        // Pick most central coor.
        List<Vector2> spawnCoors = new List<Vector2>();
        Vector2 central = samples.MinBy(s => s.sqrMagnitude);
        spawnCoors.Add(central);
        samples.Remove(central);
        // Add connected coors until done.
        int n = 5;
        while (samples.Count > 0 && spawnCoors.Count < n) {
            for (int i = 0; i < samples.Count; i++) {
                bool found = false;
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
        }
        // Spawn.
        foreach (Vector2 spawnCoor in spawnCoors) {
            GameObject emitter = Instantiate(prefabEmitter);
            emitter.transform.localPosition = spawnCoor;
        }
    }

    void Update() {
        
    }
}

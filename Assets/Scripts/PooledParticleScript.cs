using System.Collections.Generic;
using UnityEngine;

public class PooledParticleScript : MonoBehaviour
{
    static Dictionary<PooledParticleType, PooledParticleScript> DICT;
    public static void Trigger(PooledParticleType type, Vector2 position, Quaternion rotation) {
        DICT[type].Trigger(position, rotation);
    }

    public PooledParticleType type;
    ParticleSystem particles;

    void Start() {
        if (DICT == null) DICT = new();
        Debug.Assert(!DICT.ContainsKey(type), "Duplicate pooled particle.");
        DICT[type] = this;
        particles = GetComponent<ParticleSystem>();
    }
    public void Trigger(Vector2 position, Quaternion rotation) {
        transform.localPosition = position;
        transform.localRotation = rotation;
        particles.Emit((int)particles.emission.GetBurst(0).count.constant);
    }
}

public enum PooledParticleType {
    BulletBlocked, EmitterDamage,
}

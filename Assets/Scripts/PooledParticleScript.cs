using System.Collections.Generic;
using UnityEngine;

public class PooledParticleScript : MonoBehaviour
{
    static Dictionary<PooledParticleType, PooledParticleScript> DICT;
    public static void Trigger(PooledParticleType type, Vector2 position, Quaternion rotation) {
        DICT[type].Trigger(position, rotation);
    }
    public static void TriggerScaledCircle(PooledParticleType type, Vector2 position, Quaternion rotation, float scale) {
        DICT[type].TriggerScaledCircle(position, rotation, scale);
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
        var ep = new ParticleSystem.EmitParams();
        ep.position = position;
        ep.rotation3D = rotation.eulerAngles;
        ep.applyShapeToPosition = true; // shape module offsets apply relative to ep.position
        particles.Emit(ep, (int)particles.emission.GetBurst(0).count.constant);
    }
    public void TriggerScaledCircle(Vector2 position, Quaternion rotation, float scale) {
        var shape = particles.shape;
        shape.radius = scale;
        var burstCount = particles.emission.GetBurst(0).count;
        float oldConstant = burstCount.constant;
        burstCount.constant = oldConstant * scale;
        Trigger(position, rotation);
        burstCount.constant = oldConstant;
    }
}

public enum PooledParticleType {
    BulletBlocked, EmitterDamage, PulseDissolve
}

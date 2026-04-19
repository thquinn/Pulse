using System.Collections.Generic;
using UnityEngine;

public class EmitterScript : MonoBehaviour {
    static Dictionary<(EmitterScript, EmitterScript), int> triggerCounts = new(); // Increments when one emitter in a pair triggers another.

    static int PULSES_TO_LINK = 4;

    public GameObject prefabPulse, prefabLink;
    public SpriteRenderer sr;
    public AudioSource sfxHit, sfxPulse, sfxDie;

    public float hp, cooldown;
    public float vfxDamageVelocity, vfxDamageDampTime, vfxDamageCooldown;
    [HideInInspector] public bool noPulseOnDeath;

    float maxHP;
    float timer;
    int pulseCount;
    Vector3 vScale;
    float vfxDamageLastTime;

    void Start() {
        maxHP = hp;
        timer = cooldown;
    }

    void Update() {
        timer = Mathf.Min(timer + Time.deltaTime, cooldown);
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Revealed", timer / cooldown + .001f);
        sr.SetPropertyBlock(propertyBlock);
        transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref vScale, vfxDamageDampTime);
    }

    public void Pulsed(EmitterScript parentEmitter) {
        if (parentEmitter == this) return;
        if (timer >= cooldown) {
            EmitPulse();
            // Link management.
            if (parentEmitter != null) {
                (EmitterScript, EmitterScript) pair = GetPair(this, parentEmitter);
                int triggerCount = triggerCounts.ContainsKey(pair) ? triggerCounts[pair] + 1 : 1;
                triggerCounts[pair] = triggerCount;
                if (triggerCount == PULSES_TO_LINK) {
                    Instantiate(prefabLink).GetComponent<LinkScript>().Init(pair);
                }
            }
            timer = 0;
        }
    }
    static (EmitterScript, EmitterScript) GetPair(EmitterScript a, EmitterScript b) {
        Debug.Assert(a != b, "Forming duplicate emitter pair.");
        return a.GetHashCode() < b.GetHashCode() ? (a, b) : (b, a);
    }
    void EmitPulse() {
        GameObject pulse = Instantiate(prefabPulse);
        pulse.GetComponent<PulseScript>().Init(this);
        pulse.transform.localPosition = transform.localPosition;
        pulse.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        pulseCount++;
        sfxPulse.pitch = Random.Range(1f, 2f);
        sfxPulse.Play();
    }
    public void Damage(int amount) {
        if (hp <= 0) return;
        hp -= amount;
        sfxHit.pitch = 1 + 0.4f * (1 - hp / maxHP);
        sfxHit.PlayOneShot(sfxHit.clip);
        if (hp <= 0) {
            if (timer >= cooldown && !noPulseOnDeath) {
                EmitPulse();
            }
            WaveControllerScript.instance.ScoreEmitterKill();
            PooledParticleScript.Trigger(PooledParticleType.EmitterDie, transform.localPosition, Quaternion.identity);
            sfxDie.Play();
            sfxDie.transform.SetParent(null);
            Destroy(sfxDie.gameObject, 5);
            Destroy(gameObject);
            return;
        }
        float timeSinceLastDamage = Time.time - vfxDamageLastTime;
        if (timeSinceLastDamage >= vfxDamageCooldown) {
            vScale = new Vector3(-vfxDamageVelocity, -vfxDamageVelocity, 0);
            vfxDamageLastTime = Time.time;
        }
    }
}

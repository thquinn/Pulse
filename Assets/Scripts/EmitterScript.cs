using Assets.Code;
using System.Collections.Generic;
using UnityEngine;

public class EmitterScript : MonoBehaviour {
    static Dictionary<(EmitterScript, EmitterScript), int> triggerCounts = new(); // Increments when one emitter in a pair triggers another.

    static int PULSES_TO_LINK = 4;

    public GameObject prefabPulse, prefabLink, prefabPreboid, prefabBoid;
    public SpriteRenderer sr;
    public AudioSource sfxHit, sfxPulse, sfxDie;

    public float hp, cooldown, noCollisionTime;
    public float vfxSpawnSpeed, vfxDamageVelocity, vfxDamageDampTime, vfxDamageCooldown;
    [HideInInspector] public bool noPulseOnDeath;

    float maxHP;
    float timer;
    int pulseCount;
    [HideInInspector] public Dictionary<EmitterScript, LinkScript> links;
    Vector3 vScale;
    float spawnAnimT;
    float vfxDamageLastTime;

    void Start() {
        maxHP = hp;
        timer = cooldown;
        links = new();
        transform.localScale = Vector3.zero;
    }

    void Update() {
        timer = Mathf.Min(timer + Time.deltaTime, cooldown);
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Revealed", timer / cooldown + .001f);
        sr.SetPropertyBlock(propertyBlock);
        if (spawnAnimT == 1) {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref vScale, vfxDamageDampTime);
        } else {
            spawnAnimT = Mathf.Min(spawnAnimT + Time.deltaTime * vfxSpawnSpeed, 1);
            float scale = EasingFunctions.EaseOutBack(0, 1, spawnAnimT);
            transform.localScale = new Vector3(scale, scale, 1);
        }
        noCollisionTime = Mathf.Max(0, noCollisionTime - Time.deltaTime);
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
                    LinkScript link = Instantiate(prefabLink).GetComponent<LinkScript>();
                    link.Init(pair);
                    links[parentEmitter] = link;
                    parentEmitter.links[this] = link;
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
        foreach (EmitterScript linkedEmitter in links.Keys) {
            if (linkedEmitter == null) continue;
            Vector2 direction = (linkedEmitter.transform.localPosition - transform.localPosition).normalized;
            Vector2 offsetDirection = Quaternion.Euler(0, 0, -20) * direction;
            Vector2 spawnPosition = (Vector2) transform.localPosition + offsetDirection * 1.5f;
            GameObject preboid = Instantiate(prefabPreboid);
            preboid.GetComponent<PreboidScript>().Init(spawnPosition, direction, linkedEmitter, links[linkedEmitter]);
        }
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
    public void SpawnBoid() {
        Vector2 randomDirection = Random.onUnitCircle;
        Vector2 position = (Vector2) transform.localPosition + randomDirection;
        Instantiate(prefabBoid, position, Quaternion.Euler(0, 0, Mathf.Atan2(randomDirection.y, randomDirection.x) * Mathf.Rad2Deg));
    }
}

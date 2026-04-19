using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EmitterScript : MonoBehaviour {
    static Dictionary<(EmitterScript, EmitterScript), int> triggerCounts = new(); // Increments when one emitter in a pair triggers another.

    public GameObject prefabPulse, prefabLink;
    public SpriteRenderer sr;

    public float hp, cooldown;
    public float vfxDamageVelocity, vfxDamageDampTime, vfxDamageCooldown;

    float timer;
    int pulseCount;
    Vector3 vScale;
    float vfxDamageLastTime;

    void Start() {
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
                if (triggerCount == 5) {
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
    }
    public void Damage(int amount) {
        if (hp <= 0) return;
        hp -= amount;
        if (hp <= 0) {
            if (timer >= cooldown) {
                EmitPulse();
            }
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

using Assets.Code;
using UnityEngine;

public class PreboidScript : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public float speed, spawnDistance;
    public float vfxSpawnSpeed;

    Vector2 direction;
    EmitterScript targetEmitter;
    LinkScript link;
    float spawnAnimT;
    Vector3 initialScale;

    public void Init(Vector2 position, Vector2 direction, EmitterScript targetEmitter, LinkScript link) {
        transform.localPosition = position;
        this.direction = direction;
        this.targetEmitter = targetEmitter;
        this.link = link;
        initialScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Update() {
        spawnAnimT = Mathf.Min(spawnAnimT + Time.deltaTime * vfxSpawnSpeed, 1);
        float scale = EasingFunctions.EaseOutBack(0, 1, spawnAnimT);
        transform.localScale = initialScale * scale;
        if (targetEmitter == null || link == null) {
            PooledParticleScript.Trigger(PooledParticleType.BoidDie, transform.localPosition, Quaternion.identity);
            Destroy(gameObject);
        } else if (Vector2.Distance(transform.localPosition, targetEmitter.transform.localPosition) < spawnDistance) {
            targetEmitter.SpawnBoid();
            PooledParticleScript.Trigger(PooledParticleType.PreboidDone, transform.localPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void FixedUpdate() {
        rb2d.linearVelocity = direction * speed;
    }
}

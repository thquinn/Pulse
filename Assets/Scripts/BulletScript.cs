using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    public SpriteRenderer sr;
    public Rigidbody2D rb2d;

    public float speed, lifespan;

    float t;

    void Start() {
        float zRad = transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
        rb2d.linearVelocity = new Vector2(
            Mathf.Cos(zRad) * speed,
            Mathf.Sin(zRad) * speed
        );
    }

    void Update() {
        t += Time.deltaTime;
        if (t > lifespan) {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        EmitterScript emitter = collision.gameObject.GetComponent<EmitterScript>();
        if (emitter == null) {
            PooledParticleScript.Trigger(PooledParticleType.BulletBlocked, transform.localPosition, Quaternion.Euler(0, 0, 180) * transform.localRotation);
        } else {
            emitter.Damage();
        }
        Destroy(gameObject);
    }
}

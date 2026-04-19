using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    public Sprite spriteBulletUpgraded;

    public SpriteRenderer sr;
    public Rigidbody2D rb2d;
    public Collider2D c2d;

    public float speed, lifespan;

    float t;
    bool upgraded;

    void Start() {
        float zRad = transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
        rb2d.linearVelocity = new Vector2(
            Mathf.Cos(zRad) * speed,
            Mathf.Sin(zRad) * speed
        );
        // Bullets that spawn inside of a pulse are allowed to leave it.
        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCollider(c2d, results);
        foreach (Collider2D result in results) {
            Collider2D pulseCollider = result.transform.parent?.gameObject.GetComponentInChildren<CompositeCollider2D>();
            if (pulseCollider != null) {
                Physics2D.IgnoreCollision(pulseCollider, c2d);
            }
        }
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
            emitter.Damage(upgraded ? 2 : 1);
            Vector3 normal = collision.GetContact(0).normal;
            PooledParticleScript.Trigger(PooledParticleType.EmitterDamage, transform.localPosition - normal * .2f, Quaternion.LookRotation(normal));
        }
        Destroy(gameObject);
    }
    public void OnTriggerExit2D(Collider2D collision) {
        // When a bullet leaves a pulse, it gets upgraded.
        if (!upgraded) {
            sr.sprite = spriteBulletUpgraded;
            transform.localScale *= 1.5f;
            upgraded = true;
        }
    }
}

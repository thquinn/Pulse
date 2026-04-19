using UnityEngine;

public class PulseScript : MonoBehaviour
{
    public SpriteRenderer sr;
    public Collider2D collidre;
    public CircleCollider2D collidreOuterCircle, collidreInnerCircle;

    public float lifespan, growSpeed, fadeSpeed;

    EmitterScript parentEmitter;
    float t, fade, initialColliderRadiusDelta;

    void Start() {
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        sr.SetAlpha(0);
        collidre.enabled = false;
        initialColliderRadiusDelta = collidreOuterCircle.radius - collidreInnerCircle.radius;
    }
    public void Init(EmitterScript parentEmitter) {
        this.parentEmitter = parentEmitter;
    }

    void Update() {
        t += Time.deltaTime;
        if (t > lifespan && fadeSpeed > 0) {
            fadeSpeed *= -1;
        }
        fade = Mathf.Clamp01(fade + fadeSpeed * Time.deltaTime);
        collidre.enabled = fade == 1;
        if (fade == 0) {
            Destroy(gameObject);
            return;
        }
        transform.localScale += new Vector3(growSpeed * Time.deltaTime, growSpeed * Time.deltaTime, 0);
        collidreInnerCircle.radius = collidreOuterCircle.radius - initialColliderRadiusDelta / transform.localScale.x;
        sr.SetAlpha(fade);
    }
    public void Dissolve() {
        Destroy(gameObject);
        PooledParticleScript.TriggerScaledCircle(PooledParticleType.PulseDissolve, transform.localPosition, Quaternion.identity, transform.localScale.x / 2);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        collision.gameObject.GetComponent<EmitterScript>()?.Pulsed(parentEmitter);
    }
}

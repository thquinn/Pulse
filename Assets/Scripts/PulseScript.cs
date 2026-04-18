using UnityEngine;

public class PulseScript : MonoBehaviour
{
    public SpriteRenderer sr;
    public Collider2D collidre;
    public CircleCollider2D collidreOuterCircle, collidreInnerCircle;

    public float lifespan, growSpeed, fadeSpeed;

    float t, fade, initialColliderRadiusDelta;

    void Start() {
        sr.SetAlpha(0);
        collidre.enabled = false;
        initialColliderRadiusDelta = collidreOuterCircle.radius - collidreInnerCircle.radius;
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
}

using UnityEngine;

public class BoidScript : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public LayerMask layerMaskBoids, layerMaskAvoid;

    public float speed, turnRate;
    public float flockRadius;
    public float weightAvoid;
    public Vector2 rangeAvoid;
    public float avoidSpacing;
    public int avoidCount;
    public float weightSeek;
    public Vector2 rangeSeek;
    public float weightCohesion, weightAlignment, weightSeparation;
    public Vector2 rangeSeparation;

    void FixedUpdate() {
        Vector2 desired = Vector2.zero;
        // Avoidance.
        Vector2 avoidance = Vector2.zero;
        for (int i = 0; i <= avoidCount; i++) {
            Vector2 avoidVector = Quaternion.Euler(0, 0, avoidSpacing * i) * transform.right;
            avoidance -= avoidVector * Mathf.InverseLerp(rangeAvoid.x, rangeAvoid.y, CheckAvoid(avoidVector));
            if (i > 0) {
                avoidVector = Quaternion.Euler(0, 0, avoidSpacing * -i) * transform.right;
                avoidance -= avoidVector * Mathf.InverseLerp(rangeAvoid.x, rangeAvoid.y, CheckAvoid(avoidVector));
            }
        }
        if (avoidance != Vector2.zero) {
            desired += avoidance * weightAvoid;
        }
        // Seek player.
        Vector2 towardPlayer = PlayerScript.instance.transform.localPosition - transform.localPosition;
        desired += towardPlayer.normalized * weightSeek * Mathf.InverseLerp(rangeSeek.x, rangeSeek.y, towardPlayer.magnitude);
        // Flock dynamics.
        Collider2D[] flock = Physics2D.OverlapCircleAll(transform.localPosition, flockRadius, layerMaskBoids);
        Vector2 totalPosition = Vector2.zero, totalDirection = Vector2.zero;
        foreach (Collider2D c in flock) {
            if (c.gameObject == gameObject) continue;
            totalPosition += (Vector2) c.transform.localPosition;
            totalDirection += (Vector2) c.transform.right;
            // Separation.
            Vector2 delta = transform.localPosition - c.transform.localPosition;
            float distance = Vector2.Distance(c.transform.localPosition, transform.localPosition);
            float separationFactor = weightSeparation * Mathf.InverseLerp(rangeSeparation.x, rangeSeparation.y, distance);
            desired += separationFactor * delta.normalized;
        }
        if (totalPosition != Vector2.zero) {
            desired += totalPosition.normalized * weightCohesion;
            desired += totalDirection.normalized * weightAlignment;
        }
        // Steer toward desired direction.
        if (desired != Vector2.zero) {
            float cross = transform.right.x * desired.y - transform.right.y * desired.x;
            int sign = (int) Mathf.Sign(cross);
            transform.Rotate(0, 0, sign * turnRate * Time.fixedDeltaTime);
        }
        // Set velocity.
        rb2d.linearVelocity = transform.right * speed;
    }
    float CheckAvoid(Vector2 avoidVector) {
        Vector2 position2 = transform.localPosition;
        RaycastHit2D hit = Physics2D.Raycast(position2 + avoidVector * 0.5f, avoidVector, layerMaskAvoid);
        return hit.distance;
    }
}

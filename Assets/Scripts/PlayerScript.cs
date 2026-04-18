using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject prefabPulse, prefabBullet;

    public SpriteRenderer sr;

    public float speed, accel, dashSpeedMult, dashAccelMult, dashDuration, dashCooldown, shootCooldown, shootDivergence, shootForward;

    Vector2 movement;
    float dashTimeLeft, dashCooldownLeft;
    float shootCooldownLeft;
    float vRot;
    bool debugEmitted;

    void Update() {
        // Dash.
        dashCooldownLeft = Mathf.Max(0, dashCooldownLeft - Time.deltaTime);
        if (Input.anyKeyDown) {
            if (!debugEmitted) {
                GameObject pulse = Instantiate(prefabPulse);
                pulse.transform.localPosition = transform.localPosition;
                debugEmitted = true;
            } else if (dashCooldownLeft == 0) {
                dashTimeLeft = dashDuration;
            }
        }
        // Movement.
        Vector3 movement3;
        Vector2 desired = GetStick("Horizontal", "Vertical");
        movement = Vector2.MoveTowards(movement, desired, accel * Time.deltaTime * (dashTimeLeft > 0 ? dashAccelMult : 1));
        if (dashTimeLeft == 0) {
            movement3 = movement;
        } else {
            dashTimeLeft = Mathf.Max(0, dashTimeLeft - Time.deltaTime);
            if (dashTimeLeft == 0) {
                dashCooldownLeft = dashCooldown;
            }
            movement3 = movement.normalized * dashSpeedMult;
        }
        transform.localPosition += movement3 * speed * Time.deltaTime;
        // Look.
        Vector2 look = GetStick("Horizontal2", "Vertical2");
        look.x *= -1;
        float desiredLookAngle;
        if (look == Vector2.zero) {
            desiredLookAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        } else {
            desiredLookAngle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg;
        }
        float z = Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.z, desiredLookAngle, ref vRot, .02f);
        transform.localRotation = Quaternion.Euler(0, 0, z);
        // Shoot.
        if (look != Vector2.zero && dashCooldownLeft == 0) {
            if (shootCooldownLeft > 0) {
                shootCooldownLeft -= Time.deltaTime;
            }
            if (shootCooldownLeft <= 0) {
                for (int i = -1; i <= 1; i++) {
                    float bulletRotZ = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg + i * shootDivergence;
                    Vector2 bulletPos = transform.localPosition;
                    GameObject bullet = Instantiate(prefabBullet, bulletPos, Quaternion.Euler(0, 0, bulletRotZ));
                    bullet.transform.localPosition += bullet.transform.right * shootForward;
                }
                shootCooldownLeft += shootCooldown;
            }
        }
        // Sprite.
        sr.SetAlpha(dashTimeLeft > 0 ? .25f : 1);
    }

    Vector2 GetStick(string xLabel, string yLabel) {
        float jx = Input.GetAxis(xLabel);
        float jy = Input.GetAxis(yLabel);
        Vector2 v = new Vector2(jx, jy);
        if (v != Vector2.zero) {
            if (v.sqrMagnitude > 1) {
                v.Normalize();
            }
            return v;
        }
        float x = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? -1 : 0);
        float y = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);
        v = new Vector2(x, y);
        if (v.sqrMagnitude > 1) {
            v.Normalize();
        }
        return v;
    }
}

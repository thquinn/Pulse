using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    public GameObject prefabBullet;

    public BorderScript borderScript;
    public SpriteRenderer sr;
    public Rigidbody2D rb2d;

    public float speed, accel, dashSpeedMult, dashAccelMult, dashDuration, dashInvincibilityDuration, dashCooldown;
    public float shootCooldown, shootDivergence, shootForward, hitStun, hitInvincibility, hitKnockback, knockbackDecel;

    Vector2 movement, knockback;
    float dashTimeLeft, dashInvincibilityLeft, dashCooldownLeft;
    float shootCooldownLeft;
    float hitStunLeft, hitInvincibilityLeft;
    float vRot;

    void Start() {
        instance = this;
    }

    void Update() {
        // Dash.
        dashCooldownLeft = Mathf.Max(0, dashCooldownLeft - Time.deltaTime);
        if (Input.anyKeyDown && dashTimeLeft == 0 && dashCooldownLeft == 0 && movement.sqrMagnitude > .05f) {
            dashTimeLeft = dashDuration;
            dashInvincibilityLeft = dashInvincibilityDuration;
            for (float f = 0; f < dashDuration; f += .025f) {
                Invoke("LeaveShadow", f);
            }
            SFXScript.SFXDash();
            WaveControllerScript.instance.started = true;
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
        transform.localPosition += (Vector3)knockback * Time.deltaTime;
        knockback = Vector2.MoveTowards(knockback, Vector2.zero, knockbackDecel * Time.deltaTime);
        transform.localPosition = new Vector3(
            Mathf.Clamp(transform.localPosition.x, -borderScript.size.x / 2, borderScript.size.x / 2),
            Mathf.Clamp(transform.localPosition.y, -borderScript.size.y / 2, borderScript.size.y / 2),
            transform.localPosition.z
        );
        // Look.
        Vector2 look = GetStick("Horizontal2", "Vertical2");
        look.x *= -1;
        float desiredLookAngle = -999;
        if (look != Vector2.zero) {
            desiredLookAngle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg;
        } else if (movement != Vector2.zero) {
            desiredLookAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        }
        if (desiredLookAngle != -999) {
            float z = Mathf.SmoothDampAngle(sr.transform.parent.localRotation.eulerAngles.z, desiredLookAngle, ref vRot, .02f);
            sr.transform.parent.localRotation = Quaternion.Euler(0, 0, z);
        }
        // Shoot.
        if (look != Vector2.zero && dashTimeLeft == 0) {
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
                SFXScript.SFXShoot();
            }
        }
        // Timers.
        hitStunLeft = Mathf.Max(0, hitStunLeft - Time.deltaTime);
        hitInvincibilityLeft = Mathf.Max(0, hitInvincibilityLeft - Time.deltaTime);
        bool invincible = dashInvincibilityLeft > 0 || hitInvincibilityLeft > 0;
        dashInvincibilityLeft = Mathf.Max(0, dashInvincibilityLeft - Time.deltaTime);
        rb2d.simulated = !invincible;
        // Sprite.
        sr.SetAlpha(invincible ? .25f : 1);
        if (hitInvincibilityLeft > 0 && Time.time % 0.2f < 0.1f) {
            sr.SetAlpha(0);
        }
    }

    Vector2 GetStick(string xLabel, string yLabel) {
        if (hitStunLeft > 0) return Vector2.zero;
        float jx = Input.GetAxis(xLabel);
        float jy = Input.GetAxis(yLabel);
        Vector2 v = new Vector2(jx, jy);
        if (v.sqrMagnitude < .2f) {
            return Vector2.zero;
        }
        if (v.sqrMagnitude > 1) {
            v.Normalize();
        }
        return v;
    }
    public float GetDashCooldown() {
        return dashCooldownLeft / dashCooldown;
    }

    void LeaveShadow() {
        Instantiate(gameObject).GetComponent<PlayerScript>().TurnIntoShadow();
    }
    public void TurnIntoShadow() {
        gameObject.AddComponent<DashShadowScript>();
        Destroy(gameObject.GetComponentInChildren<DashCooldownScript>().gameObject);
        Destroy(this);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        EmitterScript emitter = collision.GetComponent<EmitterScript>();
        if (emitter != null) {
            if (emitter.noCollisionTime > 0) return;
            Vector2 knockbackDirection = (transform.localPosition - collision.transform.localPosition).normalized;
            knockback = knockbackDirection * hitKnockback;
        }
        GetHit();
    }
    bool GetHit() {
        if (dashTimeLeft > 0 || hitInvincibilityLeft > 0) return false;
        movement = Vector2.zero;
        hitStunLeft = hitStun;
        hitInvincibilityLeft = hitInvincibility;
        WaveControllerScript.instance.GotHit();
        SFXScript.SFXPlayerHurt();
        return true;
    }
}

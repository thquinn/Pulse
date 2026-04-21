using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    public GameObject prefabBullet;

    public Camera cam;
    public BorderScript borderScript;
    public SpriteRenderer sr;
    public Rigidbody2D rb2d;

    public float speed, accel, dashSpeedMult, dashAccelMult, dashDuration, dashInvincibilityDuration, dashCooldown;
    public float shootCooldown, shootDivergence, shootForward, hitStun, hitInvincibility, hitKnockback, knockbackDecel;
    [HideInInspector] public bool anyButtonDown;
    [HideInInspector] public bool keyboardControls;

    Vector2 movement, knockback;
    float dashTimeLeft, dashInvincibilityLeft, dashCooldownLeft;
    float shootCooldownLeft;
    float hitStunLeft, hitInvincibilityLeft;
    float vRot;

    void Start() {
        Application.targetFrameRate = 60;
        instance = this;
        InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
    }

    void Update() {
        // Dash.
        dashCooldownLeft = Mathf.Max(0, dashCooldownLeft - Time.deltaTime);
        bool dashButtonDown = anyButtonDown || Mouse.current.rightButton.ReadValue() > 0;
        if (dashButtonDown && dashTimeLeft == 0 && dashCooldownLeft == 0 && movement.sqrMagnitude > .05f) {
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
        Vector2 desired = GetStick(0);
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
        Vector2 look = GetStick(1);
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
        bool shoot = !keyboardControls || Mouse.current.leftButton.ReadValue() > 0;
        if (shoot && look != Vector2.zero && dashTimeLeft == 0) {
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

    Vector2 GetStick(int index) {
        if (hitStunLeft > 0) return Vector2.zero;
        Vector2 v = Vector2.zero;
        if (Gamepad.current != null) {
            v = (index == 0 ? Gamepad.current.leftStick : Gamepad.current.rightStick).ReadValue();
        }
        if (v.sqrMagnitude < .2f) {
            if (keyboardControls) return GetKeyboardStick(index);
            return Vector2.zero;
        }
        keyboardControls = false;
        if (v.sqrMagnitude > 1) {
            v.Normalize();
        }
        return v;
    }
    Vector2 GetKeyboardStick(int index) {
        if (index == 0) {
            Vector2 stick = Vector2.zero;
            if (Keyboard.current.aKey.isPressed) stick.x--;
            if (Keyboard.current.dKey.isPressed) stick.x++;
            if (Keyboard.current.sKey.isPressed) stick.y--;
            if (Keyboard.current.wKey.isPressed) stick.y++;
            return stick.normalized;
        }
        Vector2 mouseLocation = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        return (mouseLocation - (Vector2)transform.localPosition).normalized;
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

    void OnAnyButtonPress(InputControl control) {
        if (control.device is Gamepad) {
            anyButtonDown = true;
            keyboardControls = false;
        }
        if (control.device is Keyboard || control.device is Mouse) {
            keyboardControls = true;
        }
    }
    void LateUpdate() {
        anyButtonDown = false;
    }
}

using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject prefabPulse;

    public float speed, accel;

    Vector2 movement;

    void Update() {
        Vector2 desired = GetDesiredMovement();
        movement = Vector2.MoveTowards(movement, desired, accel * Time.deltaTime);
        Vector3 movement3 = movement;
        transform.localPosition += movement3 * speed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg);
        if (Input.anyKeyDown) {
            GameObject pulse = Instantiate(prefabPulse);
            pulse.transform.localPosition = transform.localPosition;
        }
    }

    Vector2 GetDesiredMovement() {
        float jx = Input.GetAxis("Horizontal");
        float jy = Input.GetAxis("Vertical");
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

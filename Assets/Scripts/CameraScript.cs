using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript instance;

    public PlayerScript playerScript;
    public BorderScript borderScript;

    public Vector2 halfSize;
    public float dampTime;

    Vector3 v;

    void Start() {
        instance = this;
    }

    void Update()
    {
        Vector2 extents = new Vector2(
            playerScript.transform.localPosition.x / (borderScript.size.x / 2),
            playerScript.transform.localPosition.y / (borderScript.size.y / 2)
        );
        Vector3 targetPosition = playerScript.transform.localPosition;
        targetPosition.x -= extents.x * halfSize.x;
        targetPosition.y -= extents.y * halfSize.y;
        targetPosition.z = transform.localPosition.z;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref v, dampTime);
    }

    public void Reset() {
        Vector3 pos = transform.localPosition;
        pos.x = 0;
        pos.y = 0;
        transform.localPosition = pos;
    }
}

using UnityEngine;

public class BorderWallScript : MonoBehaviour {
    public SpriteRenderer srEdge;
    public Transform transformFill;

    public void Init(Vector2 position, Quaternion rotation, float length) {
        transform.localPosition = position;
        transform.localRotation = rotation;
        srEdge.size = new Vector2(1, length / srEdge.transform.localScale.x);
        transformFill.localScale = new Vector3(2, length, 1);
    }
}

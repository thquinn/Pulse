using UnityEngine;

public class BorderWallScript : MonoBehaviour {
    public SpriteRenderer srEdge;
    public Transform transformFill;

    BorderScript borderScript;
    BorderWallSide side;


    public void Init(BorderScript borderScript, BorderWallSide side) {
        this.borderScript = borderScript;
        this.side = side;
    }

    void Update() {
        Vector2 inflated = borderScript.size + new Vector2(borderScript.expand, borderScript.expand);
        if (side == BorderWallSide.Right) {
            SetDimensions(new Vector2(inflated.x / 2, 0), Quaternion.identity, borderScript.size.y + borderScript.expand - .5f);
        } else if (side == BorderWallSide.Left) {
            SetDimensions(new Vector2(-inflated.x / 2, 0), Quaternion.Euler(0, 0, 180), borderScript.size.y + borderScript.expand - .5f);
        } else if (side == BorderWallSide.Top) {
            SetDimensions(new Vector2(0, inflated.y / 2), Quaternion.Euler(0, 0, 90), borderScript.size.x + borderScript.expand - .5f);
        } else {
            SetDimensions(new Vector2(0, -inflated.y / 2), Quaternion.Euler(0, 0, 270), borderScript.size.x + borderScript.expand - .5f);
        }
    }
    void SetDimensions(Vector2 position, Quaternion rotation, float length) {
        transform.localPosition = position;
        transform.localRotation = rotation;
        srEdge.size = new Vector2(1, length / srEdge.transform.localScale.x);
        transformFill.localScale = new Vector3(2, length, 1);
    }
}

public enum BorderWallSide {
    Left, Right, Top, Bottom
}

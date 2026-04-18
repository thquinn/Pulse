using UnityEngine;

public class LinkScript : MonoBehaviour
{
    EmitterScript parentA, parentB;

    public void Init((EmitterScript, EmitterScript) pair) {
        parentA = pair.Item1;
        parentB = pair.Item2;
        transform.localPosition = (parentA.transform.localPosition + parentB.transform.localPosition) / 2;
        Vector2 delta = parentB.transform.localPosition - parentA.transform.localPosition;
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        transform.localScale = new Vector3(delta.magnitude - 1.7f, 1, 1);
    }

    void Update() {
        if (parentA == null || parentB == null) {
            Destroy(gameObject);
            return;
        }
    }
}

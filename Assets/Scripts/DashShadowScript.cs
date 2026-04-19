using UnityEngine;
using UnityEngine.UI;

public class DashShadowScript : MonoBehaviour
{
    SpriteRenderer sr;

    void Start() {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.SetAlpha(0.25f);
    }

    void Update() {
        sr.SetAlpha(sr.color.a - 3.0f * Time.deltaTime);
        if (sr.color.a <= 0) {
            Destroy(gameObject);
        }
    }
}

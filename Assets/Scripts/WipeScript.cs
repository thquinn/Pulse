using UnityEngine;
using UnityEngine.SceneManagement;

public class WipeScript : MonoBehaviour
{
    public RectTransform rt;

    Vector2 initialPosition;
    bool leaving;
    Vector2 v;

    void Start() {
        initialPosition = rt.anchoredPosition;
    }

    void Update() {
        rt.anchoredPosition = Vector2.SmoothDamp(rt.anchoredPosition, leaving ? -initialPosition : Vector2.zero, ref v, .25f, Mathf.Infinity, Time.unscaledDeltaTime);
        if (!leaving && rt.anchoredPosition.sqrMagnitude < 10) {
            WaveControllerScript.instance.Restart();
            leaving = true;
        } else if (leaving && Vector2.Distance(-initialPosition, rt.anchoredPosition) < 10) {
            Destroy(gameObject);
        }
    }
}

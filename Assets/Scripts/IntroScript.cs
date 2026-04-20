using UnityEngine;

public class IntroScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    float vAlpha;

    void Start() {
        canvasGroup.alpha = 1;
    }

    void Update() {
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, WaveControllerScript.instance.started ? 0 : 1, ref vAlpha, 0.2f);
    }
}

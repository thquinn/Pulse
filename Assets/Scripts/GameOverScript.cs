using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverScript : MonoBehaviour
{
    public GameObject prefabWipe;

    public Canvas canvas;
    public CanvasGroup canvasGroup;

    float vAlpha;
    bool restarting;

    void Start() {
        canvasGroup.alpha = 0;
    }

    void Update() {
        bool gameOver = WaveControllerScript.instance.gameOver;
        if (!gameOver) {
            canvasGroup.alpha = 0;
            restarting = false;
        }
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, gameOver ? 1 : 0, ref vAlpha, 0.2f, Mathf.Infinity, Time.unscaledDeltaTime);
        if (!restarting && PlayerScript.instance.anyButtonDown && canvasGroup.alpha > .99f) {
            Instantiate(prefabWipe, canvas.transform);
            restarting = true;
        }
    }
}

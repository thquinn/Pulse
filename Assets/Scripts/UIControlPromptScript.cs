using UnityEngine;

public class UIControlPromptScript : MonoBehaviour
{
    public GameObject controller, keyboard;

    void Update() {
        bool keyboardControls = PlayerScript.instance.keyboardControls;
        controller.SetActive(!keyboardControls);
        keyboard.SetActive(keyboardControls);
    }
}

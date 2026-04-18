using UnityEditor.Rendering;
using UnityEngine;

public class EmitterScript : MonoBehaviour {
    public GameObject prefabPulse;
    public Material materialPulse;
    public SpriteRenderer sr;

    public float cooldown;

    float timer;

    void Start() {
        timer = cooldown;
    }

    void Update() {
        timer = Mathf.Min(timer + Time.deltaTime, cooldown);
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Revealed", timer / cooldown);
        sr.SetPropertyBlock(propertyBlock);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (timer >= cooldown) {
            GameObject pulse = Instantiate(prefabPulse);
            pulse.transform.localPosition = transform.localPosition;
            timer = 0;
        }
    }
}

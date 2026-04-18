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
        propertyBlock.SetFloat("_Revealed", timer / cooldown + .001f);
        sr.SetPropertyBlock(propertyBlock);
    }

    public void Pulsed() {
        if (timer >= cooldown) {
            GameObject pulse = Instantiate(prefabPulse);
            pulse.transform.localPosition = transform.localPosition;
            timer = 0;
        }
    }
    public void Damage() {

    }
}

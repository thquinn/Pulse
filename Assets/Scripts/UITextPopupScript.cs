using TMPro;
using UnityEngine;

public class UITextPopupScript : MonoBehaviour
{
    public RectTransform rt;
    public TMP_Text tmp;

    float t;
    Vector2 v;
    float vAlpha;
    
    void Start() {
        tmp.SetAlpha(0);
    }

    void Update() {
        t += Time.deltaTime;
        int index = transform.GetSiblingIndex() - transform.parent.childCount + 1;
        float targetY = rt.sizeDelta.y * index;
        rt.anchoredPosition = Vector2.SmoothDamp(rt.anchoredPosition, new Vector2(0, targetY), ref v, .1f);
        float targetAlpha = t > 3 ? 0 : 1;
        tmp.SetAlpha(Mathf.SmoothDamp(tmp.color.a, targetAlpha, ref vAlpha, .1f));
        if (tmp.color.a < .01f && targetAlpha == 0) {
            Destroy(gameObject);
        }
    }
}

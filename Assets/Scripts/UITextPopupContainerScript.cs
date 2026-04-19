using TMPro;
using UnityEngine;

public class UITextPopupContainerScript : MonoBehaviour
{
    public GameObject prefabTextPopup;

    public bool rightAligned;

    public void AddPopup(string text) {
        GameObject popup = Instantiate(prefabTextPopup, transform);
        TMP_Text tmp = popup.GetComponent<TMP_Text>();
        tmp.text = text;
        if (rightAligned) {
            popup.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
            tmp.alignment = TextAlignmentOptions.TopRight;
        }
    }
}

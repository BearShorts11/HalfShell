using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Vector3 originalScale;
    Vector3 expandedScale;
    RectTransform rectTransform;

    public bool SetTextColor;
    public Color Hover;
    public Color Click;
    public Color Normal;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        expandedScale = new Vector3(originalScale.x + 0.1f, originalScale.y + 0.1f, originalScale.z + 0.1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = expandedScale;
        if (SetTextColor) transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Hover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = originalScale;
        if (SetTextColor) transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Normal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SetTextColor) transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Click;
    }
}

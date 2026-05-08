using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    float originalScale;
    float expandedScale;
    RectTransform rectTransform;

    public bool SetTextColor;
    public Color Hover;
    public Color Click;
    public Color Normal;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale.x;
        expandedScale = originalScale + 0.1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(expandedScale, expandedScale, expandedScale);
        if (SetTextColor) transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Hover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(originalScale, originalScale, originalScale);
        if (SetTextColor) transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Normal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SetTextColor) transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Click;
    }
}

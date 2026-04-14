using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Display a Message on Screen for a set time
public class UI_Message : MonoBehaviour
{
    private TextMeshProUGUI messageText;

    private void Start()
    {
        messageText = GetComponent<TextMeshProUGUI>();
        messageText.text = string.Empty;
    }
    public void SetMessage(string message)
    {
        SetMessage(message, 6f);
    }
    public void SetMessage(string message, float time)
    {
        if (messageText == null) return;

        if (this.IsInvoking(nameof(ClearMessage)))
            CancelInvoke(nameof(ClearMessage));
        messageText.text = message;
        Invoke(nameof(ClearMessage), time);
    }

    void ClearMessage()
    {
        messageText.text = string.Empty;
    }
}

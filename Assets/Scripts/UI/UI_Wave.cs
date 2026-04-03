using TMPro;
using UnityEngine;

// Display the Wave and the number of waves the player is in
public class UI_Wave : MonoBehaviour
{
    private TextMeshProUGUI waveCounter;
    public string waveText = "Wave: ";
    public int waveCount = 0;

    void Start()
    {
        waveCounter = GetComponent<TextMeshProUGUI>();
        GameObject parentCanvas = this.transform.parent.gameObject;
        this.transform.parent = GameObject.Find("HUD").transform;
        Destroy(parentCanvas);
        waveCounter.text = waveText + waveCount;    
    }

    public void UpdateWave()
    {
        waveCounter.text = waveText + waveCount;
    }

    
}

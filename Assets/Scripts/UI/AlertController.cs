using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AlertController : MonoBehaviour
{
    [SerializeField] Sprite ArmorImage;
    [SerializeField] Sprite HealthImage;
    [SerializeField] Sprite HalfShellImage;
    [SerializeField] Sprite SlugImage;
    [SerializeField] Sprite FireShellImage;

    [SerializeField] Image AlertImage;
    [SerializeField] TextMeshProUGUI AlertText;

    private static Sprite _armorImage;
    private static Sprite _healthImage;
    private static Sprite _halfshellImage;
    private static Sprite _slugImage;
    private static Sprite _fireshellImage;

    private static Image _alertImage;
    private static TextMeshProUGUI _alertText;

    [SerializeField] Image RackImage;
    [SerializeField] TextMeshProUGUI RackText;

    private static Image _rackImage;
    private static TextMeshProUGUI _rackText;

    /// <summary>
    /// alert length on screen in seconds
    /// </summary>
    [SerializeField] float AlertLength;
    private static float _alertLength;
    private static bool alertOn;
    private static float alertOffTime;

    [SerializeField] float RackAlertLength;
    private static float _rackAlertLength;
    private static bool rackAlertOn;
    private static float rackAlertOffTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _armorImage = ArmorImage;
        _healthImage = HealthImage;
        _halfshellImage = HalfShellImage;
        _slugImage = SlugImage;
        _fireshellImage = FireShellImage;
        _alertImage = AlertImage;
        _alertText = AlertText;
        _alertLength = AlertLength;

        _alertImage.gameObject.SetActive(false);
        _alertText.gameObject.SetActive(false);

        _rackImage = RackImage;
        _rackText = RackText;
        _rackAlertLength = RackAlertLength;

        _rackImage.gameObject.SetActive(false);
        _rackText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (alertOn && alertOffTime <= Time.time)
        {
            alertOn = false;
            _alertImage.gameObject.SetActive(false);
            _alertText.gameObject.SetActive(false);
        }

        Debug.Log(rackAlertOn + " from update");
        if (rackAlertOn && rackAlertOffTime <= Time.time)
        {
            Debug.Log("turning rack alert off");
            rackAlertOn = false;
            _rackImage.gameObject.SetActive(false);
            _rackText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// valid strings: "health" "armor" "half shell" "slug" "fire shell" case insensitive
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    public static void SetAlert(string type, int amount)
    {
        switch (type.ToLower())
        {
            case "health":
                _alertImage.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                _alertImage.sprite = _healthImage;
                break;
            case "armor":
                _alertImage.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                _alertImage.sprite = _armorImage;
                break;
            case "half shell":
                _alertImage.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                _alertImage.sprite = _halfshellImage;
                break;
            case "slug":
                _alertImage.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 25);
                _alertImage.sprite = _slugImage;
                break;
            case "fire shell":
                _alertImage.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 25);
                _alertImage.sprite = _fireshellImage;
                break;
        }
        _alertText.text = $"+{amount}";


        _alertImage.gameObject.SetActive(true);
        _alertText.gameObject.SetActive(true);
        alertOn = true;
        alertOffTime = Time.time + _alertLength;
    }

    public static void SetRackAlert(ShellBase.ShellType type)
    {
        if (type == ShellBase.ShellType.HalfShell) return;

        //Debug.Log("setting rack alert");
        _rackText.text = "+1";

        switch (type)
        {
            case ShellBase.ShellType.Slug:
                _rackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);
                _rackImage.sprite = _slugImage;
                break;
            case ShellBase.ShellType.Incindiary:
                _rackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);
                _rackImage.sprite = _fireshellImage;
                break;
            default:
                //_rackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
                //_rackImage.sprite = _halfshellImage;
                break;
        }

        //Debug.Log("got here");
        _rackImage.gameObject.SetActive(true);
        _rackText.gameObject.SetActive(true);
        rackAlertOn = true;
        rackAlertOffTime = Time.time + _rackAlertLength;

        Debug.Log(rackAlertOn + " from method");
    }
}

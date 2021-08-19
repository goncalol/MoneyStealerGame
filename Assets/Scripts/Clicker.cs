using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Clicker : MonoBehaviour, IPointerDownHandler
{
    public float maxTimer = 0.4f;

    public int reachLimit = 1;
    public int maxClicksPerson = 13;
    public TextMeshProUGUI coinText;
    public GameObject CancelButton;

    private PlayerController player;
    private int mouseClicks;
    private float _timer;
    private static Clicker _instance;
    private int mouseClicksOnePerson;
    private int totalCoins;
    private bool isDoublingCoins;
    private AudioSource shakerSound;

    public static Clicker Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            SetActive(false);
        }
    }

    void Start()
    {
        player = PlayerController.Instance;
        shakerSound = MusicController.Instance.ShakeSound;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= maxTimer)
        {
            if (mouseClicks > reachLimit && !player.IsShaking)
            {
                player.Shake(true);

            }
            else if (mouseClicks <= reachLimit && player.IsShaking)
            {
                player.Shake(false);
            }
            _timer = 0f;
            mouseClicks = 0;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseClicks++;
        mouseClicksOnePerson++;
        if (isDoublingCoins) totalCoins += 2;
        else totalCoins++;
        coinText.SetText(totalCoins.ToString());
        if (mouseClicksOnePerson == maxClicksPerson)
        {
            SetActive(false);
            player.StartRun(true);
            GameController.Instance.CheckForBonusToAdd();
        }
        if (player.IsShaking) { 
            EZCameraShake.CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, 1f);
            shakerSound.Play();
        }
    }

    public void AddCoin()
    {
        totalCoins++;
        coinText.SetText(totalCoins.ToString());
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        CancelButton.SetActive(active);
        if (!active) { 
            mouseClicksOnePerson = 0;
            transform.GetChild(0).gameObject.SetActive(false);
        }else if (Instructions.Instance.IsShowingPersonAreaAndTap()) transform.GetChild(0).gameObject.SetActive(true);
    }
    public bool IsActive() => gameObject.activeInHierarchy;

    public int GetTotalCoins() => totalCoins;

    public void DoubleCoins(bool isDouble)
    {
        isDoublingCoins = isDouble;
    }

    public void CancelClicker()
    {
        SetActive(false);
        PlayerController.Instance.StartRun();
    }
}

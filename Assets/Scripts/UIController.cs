using Assets.Scripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject InGameUI;
    public GameObject OffGameUI;
    public GameObject UpgradeButton;
    public GameObject SettingsButton;
    public GameObject HighScoreDisplay;
    public ParticleSystem ConfetiPS;

    public TextMeshProUGUI totalCoinText;
    public TextMeshProUGUI highscore;
    public TextMeshProUGUI gameoverTotalCoinText;
    public TextMeshProUGUI gameoverCurrentCoins;
    public GameObject UpgradesDisplay;
    public GameObject GameoverDisplay;
    public GameObject SettingsDisplay;
    public Sprite BallSprite;
    public Sprite[] MusicSprite;
    public Sprite[] SoundSprite;

    public TextMeshProUGUI FPS;

    private int TotalCoins;
    private int Highscore;
    private int currentCoins;
    private int MusicOn;
    private int SoundOn;

    private int UpgradeInvisible;
    private int UpgradeMap;
    private int UpgradeCoin;
    private int UpgradeJump;

    //around 1500 coins - 10 min gameplay
    private int[] UpgradePrice = new int[] { 4500, 9000, 45000, 90000 };//30min, 1h, 5h, 10h


    private int[] UpgradeMapActiveTimes = new int[] { 30, 40, 50, 60, 70 };
    private int[] UpgradeInvisibleActiveTimes = new int[] { 15, 25, 35, 45, 55 };
    private int[] UpgradeCoinsActiveTimes = new int[] { 15, 25, 35, 45, 55 };
    private int[] UpgradeJumpQuantity = new int[] { 1, 2, 3, 4, 5 };

    private Transform invisibleBalls;
    private Transform mapBalls;
    private Transform coinBalls;
    private Transform jumpBalls;

    private static UIController _instance;

    [HideInInspector]
    public static UIController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        TotalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        Highscore = PlayerPrefs.GetInt("MaxScore", 0);

        highscore.SetText(Highscore.ToString());
        totalCoinText.SetText(TotalCoins.ToString());

        UpgradeInvisible = PlayerPrefs.GetInt("UpgradeInvisible", 0);
        UpgradeMap = PlayerPrefs.GetInt("UpgradeMap", 0);
        UpgradeCoin = PlayerPrefs.GetInt("UpgradeCoin", 0);
        UpgradeJump = PlayerPrefs.GetInt("UpgradeJump", 0);

        MusicOn = PlayerPrefs.GetInt("MusicOn", 1);
        SoundOn = PlayerPrefs.GetInt("SoundOn", 1);

        var settingsDisplay = SettingsDisplay.transform.GetChild(1);
        settingsDisplay.GetChild(5).GetComponent<Image>().sprite = MusicSprite[MusicOn];
        settingsDisplay.GetChild(6).GetComponent<Image>().sprite = SoundSprite[SoundOn];
        settingsDisplay.GetChild(8).GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("ShowInstructions", 1) == 1;

        UpdateUpgradeData();

        UpgradeNotification();
    }


    private void UpdateUpgradeData()
    {
        var display = UpgradesDisplay.transform.GetChild(1);
        invisibleBalls = display.transform.GetChild(1);
        mapBalls = display.transform.GetChild(2);
        coinBalls = display.transform.GetChild(3);
        jumpBalls = display.transform.GetChild(4);

        FillBallsAndSetPrices(UpgradeInvisible, invisibleBalls);
        FillBallsAndSetPrices(UpgradeMap, mapBalls);
        FillBallsAndSetPrices(UpgradeCoin, coinBalls);
        FillBallsAndSetPrices(UpgradeJump, jumpBalls);
    }

    private void UpgradeNotification()
    {
        if (TotalCoins >= UpgradePrice[UpgradeInvisible] || TotalCoins >= UpgradePrice[UpgradeMap] ||
            TotalCoins >= UpgradePrice[UpgradeCoin] || TotalCoins >= UpgradePrice[UpgradeJump])
            UpgradeButton.transform.GetChild(0).gameObject.SetActive(true);
        else
            UpgradeButton.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void FillBallsAndSetPrices(int ballCount, Transform upgrade)
    {
        for (var i = 0; i < ballCount; i++)
        {
            upgrade.GetChild(i).GetComponent<Image>().sprite = BallSprite;
        }
        if (ballCount < 4)
            upgrade.GetChild(4).GetComponent<TextMeshProUGUI>().SetText(UpgradePrice[ballCount].ToString());
        else
            upgrade.GetChild(4).GetComponent<TextMeshProUGUI>().SetText("---");
    }

    public void SwitchGameMode()
    {
        OffGameUI.SetActive(false);
        InGameUI.SetActive(true);
        UpgradeButton.SetActive(false);
        SettingsButton.SetActive(false);
    }

    public void OnUpgradeClick(bool activate)
    {
        UpgradesDisplay.SetActive(activate);
        MusicController.Instance.ButtonSound.Play();
    }

    public void OnBuyUpgradeClick(int upgradeType)
    {
        if (upgradeType == 0)//invisible
        {
            if (UpgradeInvisible < 4 && TotalCoins >= UpgradePrice[UpgradeInvisible])
            {
                TotalCoins -= UpgradePrice[UpgradeInvisible];
                MusicController.Instance.BuySound.Play();
                PlayerPrefs.SetInt("TotalCoins", TotalCoins);
                totalCoinText.SetText(TotalCoins.ToString());
                UpgradeInvisible++;
                PlayerPrefs.SetInt("UpgradeInvisible", UpgradeInvisible);
                FillBallsAndSetPrices(UpgradeInvisible, invisibleBalls);
                UpgradeNotification();
            }
        }
        else if (upgradeType == 1)//map
        {
            if (UpgradeMap < 4 && TotalCoins >= UpgradePrice[UpgradeMap])
            {
                TotalCoins -= UpgradePrice[UpgradeMap];
                MusicController.Instance.BuySound.Play();
                PlayerPrefs.SetInt("TotalCoins", TotalCoins);
                totalCoinText.SetText(TotalCoins.ToString());
                UpgradeMap++;
                PlayerPrefs.SetInt("UpgradeMap", UpgradeMap);
                FillBallsAndSetPrices(UpgradeMap, mapBalls);
                UpgradeNotification();
            }

        }
        else if (upgradeType == 2)//coins
        {
            if (UpgradeCoin < 4 && TotalCoins >= UpgradePrice[UpgradeCoin])
            {
                TotalCoins -= UpgradePrice[UpgradeCoin];
                MusicController.Instance.BuySound.Play();
                PlayerPrefs.SetInt("TotalCoins", TotalCoins);
                totalCoinText.SetText(TotalCoins.ToString());
                UpgradeCoin++;
                PlayerPrefs.SetInt("UpgradeCoin", UpgradeCoin);
                FillBallsAndSetPrices(UpgradeCoin, coinBalls);
                UpgradeNotification();
            }
        }
        else//jump
        {
            if (UpgradeJump < 4 && TotalCoins >= UpgradePrice[UpgradeJump])
            {
                TotalCoins -= UpgradePrice[UpgradeJump];
                MusicController.Instance.BuySound.Play();
                PlayerPrefs.SetInt("TotalCoins", TotalCoins);
                totalCoinText.SetText(TotalCoins.ToString());
                UpgradeJump++;
                PlayerPrefs.SetInt("UpgradeJump", UpgradeJump);
                FillBallsAndSetPrices(UpgradeJump, jumpBalls);
                UpgradeNotification();
            }
        }
    }

    public void ShowGameOverDisplay()
    {
        var coins = Clicker.Instance.GetTotalCoins();
        PlayerPrefs.SetInt("TotalCoins", coins + TotalCoins);

        if (coins > Highscore)
        {
            PlayerPrefs.SetInt("MaxScore", coins);
            ShowHighScore(coins);
        }
        else
            ShowIncreaseCoin(coins);
    }

    public void OnOKHighscoreClick()
    {
        HighScoreDisplay.SetActive(false);
        ShowIncreaseCoin(Clicker.Instance.GetTotalCoins());
        MusicController.Instance.ButtonSound.Play();
    }

    private void ShowHighScore(int coins)
    {
        MusicController.Instance.ApplauseSound.Play();
        HighScoreDisplay.SetActive(true);
        currentCoins = coins;
        Invoke("ShowConfetti",.5f);
    }

    private void ShowConfetti()
    {
        var group = HighScoreDisplay.transform.GetChild(1);
        group.gameObject.SetActive(true);
        group.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(currentCoins.ToString());
        ConfetiPS.Stop();
        ConfetiPS.Play();
    }

    private void ShowIncreaseCoin(int coins)
    {
        gameoverTotalCoinText.SetText(TotalCoins.ToString());
        gameoverCurrentCoins.SetText(coins.ToString());

        GameoverDisplay.SetActive(true);

        StartCoroutine("IncreaseTotalCoinRoutine", coins);

    }

    IEnumerator IncreaseTotalCoinRoutine(int coins)
    {
        var coinSound = MusicController.Instance.CoinNumberSound;
        yield return new WaitForSeconds(1);
        var total = TotalCoins + coins;
        var totalSoFar = TotalCoins;
        var increment = (coins < 100 ? 5 : coins * 5 / 100); //diminuir 5 para mais tempo <-> 5 incremento para 100 moedas
        while (coins > 0)
        {
            yield return new WaitForSeconds(.05f);
            coins -= increment;
            totalSoFar += increment;
            coinSound.Play();
            gameoverTotalCoinText.SetText((totalSoFar > total ? total : totalSoFar).ToString());
            gameoverCurrentCoins.SetText((coins < 0 ? 0 : coins).ToString());
        }
    }

    public int GetPowerUpgradedValue(PowerType map)
    {
        switch (map)
        {
            case PowerType.DoubleCoins:
                return UpgradeCoinsActiveTimes[UpgradeCoin];
            case PowerType.Map:
                return UpgradeMapActiveTimes[UpgradeMap];
            case PowerType.Invisible:
                return UpgradeInvisibleActiveTimes[UpgradeInvisible];
            case PowerType.Jump:
                return UpgradeJumpQuantity[UpgradeJump];
            default:
                return 0;
        }
    }

    public void OnSettingsClick(bool close)
    {
        SettingsDisplay.SetActive(!close);
        MusicController.Instance.ButtonSound.Play();
    }

    public void OnMusicClick()
    {
        var img = SettingsDisplay.transform.GetChild(1).GetChild(5).GetComponent<Image>();
        if (MusicOn == 1)
        {
            img.sprite = MusicSprite[0];
            MusicOn = 0;
            MusicController.Instance.MusicOn(false);
        }
        else
        {
            img.sprite = MusicSprite[1];
            MusicOn = 1;
            MusicController.Instance.MusicOn(true);
        }

        PlayerPrefs.SetInt("MusicOn", MusicOn);
    }

    public void OnSoundClick()
    {
        var img = SettingsDisplay.transform.GetChild(1).GetChild(6).GetComponent<Image>();
        if (SoundOn == 1)
        {
            img.sprite = SoundSprite[0];
            SoundOn = 0;
            MusicController.Instance.SoundOn(false);
        }
        else
        {
            img.sprite = SoundSprite[1];
            SoundOn = 1;
            MusicController.Instance.SoundOn(true);
        }

        PlayerPrefs.SetInt("SoundOn", SoundOn);
    }

    public void OnTutorialToggle(bool check)
    {
        Instructions.Instance.Activate(check);
    }
}

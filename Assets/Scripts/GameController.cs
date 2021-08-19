using Assets.Scripts.Powers;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public PoliceController[] police;
    public Transform[] policeSpots;
    public GameObject StarGroup;
    public GameObject minimap;
    public GameObject minimapCamera;
    public GameObject[] PowerPrefabs;
    public GameObject Flash;
    public GameObject PowerSliderGroup;
    public GameObject BonusGroup;
    public Sprite[] BallSprites;

    private static GameController _instance;

    private int starLevel;

    //powers
    private Coroutine spwanPowerRoutine;
    private int[] timeToSpawnNewPower = new int[] { 15, 25, 35 };
    private int timePowerActiveOnMap = 20;
    private GameObject spawnPowerObj;
    private ExclamationPoint presentIcon;

    //estas vars é para o bonus que player recebe qd rouba varia pessoas num periodo de tempo
    private bool isBonusCooldown;
    private int robbedForBonus;
    private Coroutine countDownForBonus;
    private int peopleToRobToGetBonus = 5;
    private int timeToRobPeopleForBonus = 40;
    private float coolDownTime = 20f;
    private int bonusActiveTime = 10;
    private float bonusSliderStep = .1f;
    private Slider bonusSlider;
    private int totalRobbed;

    [HideInInspector]
    public static GameController Instance { get { return _instance; } }

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

    public void Start()
    {
        bonusSlider = BonusGroup.transform.GetChild(0).GetComponent<Slider>();
        MusicController.Instance.ResetPitch();
    }

    public IEnumerator SpwanPowerRoutine(int initialWaitTime)
    {
        yield return new WaitForSeconds(initialWaitTime);
        while (true)
        {
            spawnPowerObj = Instantiate(ChoosePower(), policeSpots[UnityEngine.Random.Range(0, policeSpots.Length)].position, Quaternion.identity);
            presentIcon = NotificationController.Instance.CreatePresent(spawnPowerObj.transform.GetChild(0).transform.GetChild(0).position);

            var textComponent = presentIcon.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            var time = timePowerActiveOnMap;
            while (time >= 0)
            {
                textComponent.SetText(time.ToString());
                yield return new WaitForSeconds(1);
                time--;
            }

            if (presentIcon != null) { Destroy(presentIcon.gameObject); presentIcon = null; }
            if (spawnPowerObj != null) { Destroy(spawnPowerObj); spawnPowerObj = null; }

            var waitTime = timeToSpawnNewPower[UnityEngine.Random.Range(0, timeToSpawnNewPower.Length)];
            yield return new WaitForSeconds(waitTime);
        }
    }

    //fazer a categorização de raros neste method
    private GameObject ChoosePower()
    {
        var list = PowerPrefabs;
        if (!PlayerController.Instance.IsOutOfJumps) list = list.Where(e => e.name != "JumpPowerGroup").ToArray();
        else return list.FirstOrDefault(e=>e.name== "JumpPowerGroup"); 

        return list[UnityEngine.Random.Range(0, list.Length)];
        //return PowerPrefabs.FirstOrDefault(e=>e.name== "GhostPowerGroup");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void IncreaseRobbery(Vector3 position)
    {
        if (!police.Any(e => e.isActiveAndEnabled)) return;

        float? minDistance = null;
        PoliceController policeCheckingRobbery = null;

        foreach (var p in police)
        {
            if (!p.isActiveAndEnabled) continue;
            var dist = Vector3.Distance(p.transform.position, position);
            if (!minDistance.HasValue || dist < minDistance)
            {
                minDistance = dist;
                policeCheckingRobbery = p;
            }
        }

        policeCheckingRobbery.CheckingForDisturbs(position);
    }

    private void SpawnPolice()
    {
        totalRobbed++;
        if (totalRobbed == 5)
        {
            StarIncrease();
            Instructions.Instance.PoliceInstructions();
            police[0].transform.parent.gameObject.SetActive(true);
        }
        else if (totalRobbed == 20)
        {
            timeToRobPeopleForBonus += 10;
            StarIncrease();
            police[1].transform.parent.gameObject.SetActive(true);
        }
        else if (totalRobbed == 40)
        {
            timeToRobPeopleForBonus += 10;
            StarIncrease();
            police[2].transform.parent.gameObject.SetActive(true);
        }

    }

    public void CheckForBonusToAdd()
    {
        SpawnPolice();
        if (isBonusCooldown) return;
        BonusGroup.transform.GetChild(1).GetChild(robbedForBonus).GetComponent<Image>().sprite = BallSprites[1];
        robbedForBonus++;
        if (robbedForBonus == 1) Instructions.Instance.BonusTip();
        if (robbedForBonus >= peopleToRobToGetBonus)
        {
            StopCoroutine(countDownForBonus);
            ResetBonus();
            PlayerController.Instance.AddBonus(bonusActiveTime);
            return;
        }
        if (countDownForBonus == null)
        {
            countDownForBonus = StartCoroutine("CountDownForBonusRoutine");
        }
    }

    IEnumerator CountDownForBonusRoutine()
    {
        int countDown = timeToRobPeopleForBonus;
        for (float i = countDown; i > 0; i -= bonusSliderStep)
        {
            bonusSlider.value = i / countDown;
            yield return new WaitForSeconds(bonusSliderStep);
        }
        ResetBonus();
    }

    private void ResetBonus()
    {
        HiddingBonusBalls(true);
        countDownForBonus = null;
        robbedForBonus = 0;
        isBonusCooldown = true;
        Invoke("StopBonusCooldown", coolDownTime);
    }

    private void StopBonusCooldown()
    {
        isBonusCooldown = false;
        HiddingBonusBalls(false);
        bonusSlider.value = 1;
    }

    private void HiddingBonusBalls(bool hide)
    {
        var child = BonusGroup.transform.GetChild(1);
        Color tempColor = Color.white;
        for (int i = 0; i < child.childCount; i++)
        {
            var image = child.GetChild(i).GetComponent<Image>();
            image.sprite = BallSprites[0];
            tempColor = image.color;
            tempColor.a = hide ? .5f : 1f;
            image.color = tempColor;
        }
        child.GetComponent<Image>().color = tempColor;
        BonusGroup.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().color = tempColor;
        var background = BonusGroup.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        tempColor = background.color;
        tempColor.a = hide ? .5f : 1f;
        background.color = tempColor;
    }
         

    public void HitPower()
    {
        Flash.SetActive(true);
        StopCoroutine(spwanPowerRoutine);
        if (presentIcon != null)
        {
            Destroy(presentIcon.gameObject);
            presentIcon = null;
        }
        if (spawnPowerObj != null) spawnPowerObj.transform.GetChild(0).GetComponent<Power>().Execute();
        else StartPowerRoutine();
    }

    public void StartPowerRoutine(int time = 30)
    {
        if (spawnPowerObj != null)
        {
            Destroy(spawnPowerObj);
            spawnPowerObj = null;
        }
        spwanPowerRoutine = StartCoroutine("SpwanPowerRoutine", time);
    }

    public void StopPowerRoutine()
    {
        StopCoroutine(spwanPowerRoutine);
        if (presentIcon != null) { Destroy(presentIcon.gameObject); presentIcon = null; }
        if (spawnPowerObj != null) { Destroy(spawnPowerObj); spawnPowerObj = null; }
    }

    private void StarIncrease()
    {
        starLevel++;
        StarGroup.transform.GetChild(starLevel - 1).gameObject.SetActive(true);
    }

}

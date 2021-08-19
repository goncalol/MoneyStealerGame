using System;
using System.Collections;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    public GameObject InstructionsGroup;

    private int showInstructions;
    private int instructionLevel;
    private static Instructions _instance;

    [HideInInspector]
    public static Instructions Instance { get { return _instance; } }

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
        showInstructions = PlayerPrefs.GetInt("ShowInstructions", 1);
        instructionLevel = PlayerPrefs.GetInt("InstructionLevel", 1);

        if (IsShowingPersonAreaAndTap())
            ActivatePeopleArea(true);
    }

    public void ShowInstructions()
    {
        if (showInstructions == 1 && instructionLevel == 1)
            ActivateTurnPlayerInstructions();
    }

    private void ActivateTurnPlayerInstructions()
    {
        Time.timeScale = 0;
        InstructionsGroup.SetActive(true);
        InstructionsGroup.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void OnOKTurnPlayerInstructions()
    {
        MusicController.Instance.ButtonSound.Play();
        InstructionsGroup.transform.GetChild(1).gameObject.SetActive(false);
        InstructionsGroup.transform.GetChild(7).gameObject.SetActive(true);
    }

    public void OnOKSpinPlayerInstructions()
    {
        MusicController.Instance.ButtonSound.Play();
        Time.timeScale = 1;
        InstructionsGroup.transform.GetChild(7).gameObject.SetActive(false);
        InstructionsGroup.SetActive(false);
        instructionLevel = 2;
        PlayerPrefs.SetInt("InstructionLevel", 2);

        ActivatePeopleArea(true);
    }

    public void OnOKBonusHint()
    {
        MusicController.Instance.ButtonSound.Play();
        Time.timeScale = 1;
        InstructionsGroup.transform.GetChild(2).gameObject.SetActive(false);
        InstructionsGroup.SetActive(false);
        instructionLevel = 3;
        PlayerPrefs.SetInt("InstructionLevel", 3);
    }

    public void OnOKPoliceInstructions()
    {
        if (instructionLevel == 3)
        {
            MusicController.Instance.ButtonSound.Play();
            Time.timeScale = 1;
            InstructionsGroup.transform.GetChild(3).gameObject.SetActive(false);
            InstructionsGroup.SetActive(false);
            instructionLevel = 4;
            PlayerPrefs.SetInt("InstructionLevel", instructionLevel);
            ActivatePeopleArea(false);
        }
    }

    public void OnOKJumpNotification()
    {
        if (instructionLevel == 4)
        {
            MusicController.Instance.ButtonSound.Play();
            InstructionsGroup.transform.GetChild(4).gameObject.SetActive(false);
            InstructionsGroup.transform.GetChild(5).gameObject.SetActive(true);
        }
    }

    public void OnOKPowerUpHint()
    {
        if (instructionLevel == 4)
        {
            MusicController.Instance.ButtonSound.Play();
            Time.timeScale = 1;
            InstructionsGroup.transform.GetChild(6).gameObject.SetActive(false);
            InstructionsGroup.SetActive(false);
            instructionLevel = 5;
            showInstructions = 0;
            PlayerPrefs.SetInt("InstructionLevel", 5);
            PlayerPrefs.SetInt("ShowInstructions", 0);
            GameController.Instance.StartPowerRoutine(3);
        }
    }

    public bool IsShowingPersonAreaAndTap() => IsActive() && ShowPersonAreaAndTap(); 

    public bool IsActive() => showInstructions == 1;

    public void BonusTip()
    {
        if(showInstructions == 1 && instructionLevel==2)
        {
            Time.timeScale = 0;
            InstructionsGroup.SetActive(true);
            InstructionsGroup.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void PoliceInstructions()
    {
        if(showInstructions == 1 && instructionLevel == 3)
        {
            Time.timeScale = 0;
            InstructionsGroup.SetActive(true);
            InstructionsGroup.transform.GetChild(3).gameObject.SetActive(true);

        }
    }

    public bool PoliceTackleInstructions()
    {
        if (showInstructions == 1 && instructionLevel == 4)
        {
            Time.timeScale = 0;
            PlayerController.Instance.ResetJumps();
            InstructionsGroup.SetActive(true);
            InstructionsGroup.transform.GetChild(4).gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    public void Jump()
    {
        PlayerController.Instance.JumpInst();
        Time.timeScale = 1;
        InstructionsGroup.transform.GetChild(5).gameObject.SetActive(false);
        InstructionsGroup.SetActive(false);
        StartCoroutine(HintPowers());
    }

    private IEnumerator HintPowers()
    {
        yield return new WaitForSeconds(5f);
        Time.timeScale = 0;
        InstructionsGroup.SetActive(true);
        InstructionsGroup.transform.GetChild(6).gameObject.SetActive(true);
    }

    private void ActivatePeopleArea(bool activate)
    {
        var people = GameObject.FindGameObjectsWithTag("Person");
        foreach (var p in people)
        {
            p?.GetComponent<PersonController>()?.BackAreaInstruction(activate);
        }
    }

    private bool ShowPersonAreaAndTap()
        => instructionLevel == 2 || instructionLevel == 3;

    public void Activate(bool check)
    {
        showInstructions = check ? 1 : 0;
        PlayerPrefs.SetInt("ShowInstructions", showInstructions);
        if(check && instructionLevel == 5)
        {
            instructionLevel = 1;
            PlayerPrefs.SetInt("InstructionLevel", instructionLevel);
        }
        else if (ShowPersonAreaAndTap())
            ActivatePeopleArea(check);

    }
}

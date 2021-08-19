using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource MainMusic;
    public AudioSource StartMusic;
    public AudioSource CoinNumberSound;
    public AudioSource CoinBonusSound;
    public AudioSource ShakeSound;
    public AudioSource ApplauseSound;
    public AudioSource ButtonSound;
    public AudioSource BuySound;
    public AudioSource DodgeSound;
    public AudioSource HitSound;
    public AudioSource BonusSound;
    public AudioSource JumpStartSound;
    public AudioSource CoinFallingSound;
    public AudioSource PoliceAwareSound;
    public AudioSource ShiniesSound;
    public AudioSource BackgroundSound;

    private static MusicController _instance;
    private bool isHighPitch;
    private Coroutine currentRoutine;
    private bool isMusicOn;

    [HideInInspector]
    public static MusicController Instance { get { return _instance; } }

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

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("MusicOn", 1) == 1)
            PlayMainMusic();
        else
            StopMainMusic();

        if (PlayerPrefs.GetInt("SoundOn", 1) == 1)
            SoundOn(true);
        else
            SoundOn(false);
    }

    private void PlayMainMusic()
    {
        isMusicOn = true;
        StartMusic.Play();
        MainMusic.PlayDelayed(StartMusic.clip.length);
    }

    private void StopMainMusic()
    {
        isMusicOn = false;
        StartMusic.Stop();
        MainMusic.Stop();
    }

    public void IncreasePitch()
    {
        if (!isMusicOn) return;
        isHighPitch = true;
        if(currentRoutine!=null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(IncreasePitchCO());
    }

    public void DecreasePitch(bool gameover)
    {
        if (!isMusicOn) return;
        isHighPitch = false;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DescreasePitchCO(gameover));
    }

    public void ResetPitch()
    {
        if (!isMusicOn) return;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        if (isHighPitch)
        {
            currentRoutine = StartCoroutine(ResetPitchFromHighCO());
        }
        else
        {
            currentRoutine = StartCoroutine(ResetPitchFromLowCO());
        }
    }

    private IEnumerator IncreasePitchCO()
    {
        while (MainMusic.pitch<=1.7f)
        {
            yield return new WaitForSeconds(.5f);
            MainMusic.pitch += .05f;
        }
        MainMusic.pitch = 1.7f;
    }

    private IEnumerator DescreasePitchCO(bool gameover)
    {
        var step = gameover ? .5f : .05f;
        while (MainMusic.pitch >= .5f)
        {
            yield return new WaitForSeconds(.5f);
            MainMusic.pitch -= step;
        }
        MainMusic.pitch = .5f;
    }

    private IEnumerator ResetPitchFromHighCO()
    {
        while (MainMusic.pitch > 1f)
        {
            yield return new WaitForSeconds(.5f);
            MainMusic.pitch -= .05f;
        }

        MainMusic.pitch = 1f;
    }

    private IEnumerator ResetPitchFromLowCO()
    {
        while (MainMusic.pitch < 1f)
        {
            yield return new WaitForSeconds(.5f);
            MainMusic.pitch += .05f;
        }
        MainMusic.pitch = 1f;
    }

    public void MusicOn(bool on)
    {
        if (on)
        {
            PlayMainMusic();
        }
        else
        {
            StopMainMusic();
        }
    }

    public void SoundOn(bool on)
    {
        CoinNumberSound.mute = !on;
        CoinBonusSound.mute = !on;
        ShakeSound.mute = !on;
        ApplauseSound.mute = !on;
        ButtonSound.mute = !on;
        BuySound.mute = !on;
        DodgeSound.mute = !on;
        HitSound.mute = !on;
        BonusSound.mute = !on;
        JumpStartSound.mute = !on;
        CoinFallingSound.mute = !on;
        PoliceAwareSound.mute = !on;
        ShiniesSound.mute = !on;
        BackgroundSound.mute = !on;
    }
}

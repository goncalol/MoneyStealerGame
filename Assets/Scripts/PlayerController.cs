using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 7f;
    public float playerRotationSpeed = 50f;
    public int reachLimit = 1;
    public float maxTimer = 0.4f;
    public ParticleSystem particles;
    public ParticleSystem particles_jewels;
    public ParticleSystem[] bonusParticles;
    public ParticleSystem stolePersonPS;
    public ParticleSystem HitPS;
    public Transform[] followingPoints;
    public TextMeshProUGUI jumpText;
    public GameObject holeObj;
    public GameObject ghostAnimation;

    private static PlayerController _instance;
    private Vector3 moveDirection;
    private Animator anim;
    private CharacterController controller;
    private bool isShaking;
    private GameObject objectToInteract;
    private Coroutine TimerCoroutine;
    private Coroutine accelerationRoutine;
    private Coroutine increaseBonusCoin;
    private float currCountdownValue;
    private float h1;
    private List<FollowingPoints> FollowingPointList;
    private Vector3 startJumpPos;
    private int jumpCount;
    private bool canJump;
    private bool isDoubleCoins;
    private bool move;
    private float PlayerSpeed;
    private float PlayerRotationSpeed;
    private bool playerInvisible;

    [HideInInspector]
    public bool IsWalking;

    [HideInInspector]
    public bool IsDead;

    [HideInInspector]
    public bool isJumping;

    [HideInInspector]
    public bool isIdleStart;

    public static PlayerController Instance { get { return _instance; } }


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

    void Start()
    {
        isIdleStart = true;
        move = true;
        PlayerSpeed = playerSpeed;
        PlayerRotationSpeed = playerRotationSpeed;
        FollowingPointList = followingPoints.Select(FollowingPoints.Create).ToList();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (IsWalking)
        {
            moveDirection = Vector3.forward;
            moveDirection = transform.TransformDirection(moveDirection);
            if (!isJumping && canJump && jumpCount != 0)
            {
                isJumping = true;
                canJump = false;
                jumpCount--;
                UpdateJumpCountText();
                startJumpPos = transform.position;
                MusicController.Instance.DodgeSound.Play();
                anim.SetTrigger("Jump");
                Physics.IgnoreLayerCollision(0, 9);
            }
            if (isJumping)
            {
                if (Vector3.Distance(transform.position, startJumpPos) < 10f)
                    controller.Move(moveDirection * Time.deltaTime * PlayerSpeed * 3);
                else
                    isJumping = false;
            }
            else
            {
                if(move) controller.Move(moveDirection * Time.deltaTime * PlayerSpeed);

            }

            transform.Rotate(0, h1 * PlayerRotationSpeed * Time.deltaTime, 0);
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    public void Die(Transform police)
    {
        if (IsDead) return;
        IsWalking = false;
        IsDead = true;
        controller.enabled = false;
        anim.SetTrigger("Die");
        Hit();
        Clicker.Instance.SetActive(false);
        StopCoinParticles();
        foreach (var bonusParticle in bonusParticles) bonusParticle.Stop();
        if (increaseBonusCoin != null) StopCoroutine(increaseBonusCoin);
        objectToInteract?.GetComponentInParent<PersonController>()?.ExitGrabState();
        UIController.Instance.ShowGameOverDisplay();
        GameController.Instance.StopPowerRoutine();
        MusicController.Instance.DecreasePitch(true);
        CameraController.Instance.Death();
    }

    public void OnCollidedWithEnemy(GameObject objectToInteract)
    {
        anim.SetBool("isGrabing", true);
        anim.SetBool("IsRunning", false);
        this.objectToInteract = objectToInteract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Police_Baton")
        {
            Die(other.transform.root);
        }
        else if (other.tag == "bonus")
        {
            MusicController.Instance.BonusSound.Play();
            GameController.Instance.HitPower();
        }
    }

    public void OnStartGrabState()
    {
        Clicker.Instance.SetActive(true);
        GameController.Instance.IncreaseRobbery(transform.position);

        var grabPlayerPos = objectToInteract.transform.GetChild(0);
        var grabPlayerLookAt = objectToInteract.transform.parent.parent;

        var relativePoint = grabPlayerLookAt.InverseTransformPoint(CameraController.Instance.GetStandardPos().transform.position);
        if (relativePoint.x < 0.0)
            CameraController.Instance.Tilt(true);
        else if (relativePoint.x > 0.0)
            CameraController.Instance.Tilt(false);
        else
            CameraController.Instance.Tilt(true);


        gameObject.transform.localPosition = new Vector3(grabPlayerPos.position.x, 0, grabPlayerPos.position.z);
        gameObject.transform.localRotation = grabPlayerLookAt.localRotation;
        objectToInteract.GetComponentInParent<PersonController>().StartGrabState();

        //controller.enabled = false;
        IsWalking = false;
        Physics.IgnoreLayerCollision(0, 9);
        TimerCoroutine = StartCoroutine(StartCountdown());
    }

    public void OnStartRunState()
    {
        if (TimerCoroutine != null) StopCoroutine(TimerCoroutine);
        if (objectToInteract != null)
        {
            objectToInteract.GetComponentInParent<PersonController>().ExitGrabState();
            objectToInteract = null;
        }
        isShaking = false;
        IsWalking = true;    
        anim.SetBool("isGrabing", false);
        anim.SetBool("isShaking", false);
        Physics.IgnoreLayerCollision(0, 9, false);
        CameraController.Instance.tiltMode = false;
    }

    public void OnEndShakeState()
    {
        isShaking = false;
        anim.SetBool("isShaking", isShaking);
        StopCoinParticles();
    }

    private void StopCoinParticles()
    {
        MusicController.Instance.CoinFallingSound.Stop();
        particles.Stop();
        particles_jewels.Stop();
    }

    private IEnumerator StartCountdown(float countdownValue = 3)
    {
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0)//true 
        {
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        Clicker.Instance.SetActive(false);
        objectToInteract?.GetComponentInParent<PersonController>()?.StartTackleState();
        objectToInteract = null;
        anim.SetTrigger("Fall");
        Hit();
    }

    public void StartRun(bool hasStolePerson = false)
    {
        if (hasStolePerson) {

            MusicController.Instance.ShiniesSound.Play();
            stolePersonPS.Play();
        }
        if (accelerationRoutine != null)
            StopCoroutine(accelerationRoutine);
        PlayerSpeed -= 5f;
        accelerationRoutine = StartCoroutine(IncreaseVelocity());
        anim.SetBool("IsRunning", true);
    }

    public void Turn(bool toLeft)
    {
        h1 = toLeft ? -1 : 1;
    }

    public void StopTurn()
    {
        h1 = 0;
    }

    public void SetMoving(bool m)
    {
        move = m;
        if (m)
        {
            PlayerRotationSpeed = playerRotationSpeed;
        }
        else
        {
            PlayerRotationSpeed += 70f;
        }
    }

    public void Shake(bool toShake)
    {
        isShaking = toShake;
        anim.SetBool("isShaking", isShaking);
        if (toShake)
        {
            particles.Play();
            MusicController.Instance.CoinFallingSound.Play();
            if (isDoubleCoins) particles_jewels.Play();
        }
        objectToInteract.GetComponentInParent<PersonController>().SetShake(toShake);
    }

    public Transform GetFollowPoint(string policeName)
    {
        var point = FollowingPointList.FirstOrDefault(e => e.FollowerNameReference == null);
        point.FollowerNameReference = policeName;
        return point.Position;
    }

    public void UnFollowPoint(string policeName)
    {
        var point = FollowingPointList.FirstOrDefault(e => e.FollowerNameReference == policeName);
        point.FollowerNameReference = null;
    }

    public bool IsSlowingDown()
    {
        return controller.velocity.magnitude <= PlayerSpeed - 0.5f || !IsWalking;
    }

    private void UpdateJumpCountText()
    {
        jumpText.SetText("x" + jumpCount);
    }

    public void Jump()
    {
        if (IsWalking && !isJumping && jumpCount != 0) canJump = true;
    }

    public void JumpInst()
    {
        canJump = true;
    }


    public void SetPlayerInvisible(bool isInvisible)
    {
        playerInvisible = isInvisible;
        ghostAnimation.SetActive(isInvisible);
    }

    public void ResetJumps()
    {
        jumpCount = UIController.Instance.GetPowerUpgradedValue(PowerType.Jump);
        UpdateJumpCountText();
    }

    public void AddBonus(int bonusTime)
    {
        foreach (var bonusParticle in bonusParticles) bonusParticle.Play();
        increaseBonusCoin = StartCoroutine("IncreaseBonusCoinRoutine", bonusTime);
    }

    IEnumerator IncreaseBonusCoinRoutine(int bonusTime)
    {
        var countDown = (float)bonusTime;
        var coinSound = MusicController.Instance.CoinBonusSound;
        while (countDown > 0)
        {
            Clicker.Instance.AddCoin();
            yield return new WaitForSeconds(.2f);
            coinSound.Play();
            countDown -= 0.2f;
        }
        foreach (var bonusParticle in bonusParticles) bonusParticle.Stop();
    }

    private IEnumerator IncreaseVelocity()
    {
        while (PlayerSpeed < playerSpeed)
        {
            yield return new WaitForSeconds(.2f);
            PlayerSpeed += 1f;
        }

        PlayerSpeed = playerSpeed;
    }

    public void StartPlayerMobility()
    {
        holeObj.SetActive(false);
        IsWalking = true;
        anim.SetBool("IsRunning", true);
        Instructions.Instance.ShowInstructions();
    }

    public void JumpStart()
    {
        isIdleStart = false;
        MusicController.Instance.JumpStartSound.Play();
        if (!Instructions.Instance.IsActive())
        {
            ResetJumps();
            GameController.Instance.StartPowerRoutine();
        }
        anim.SetBool("IsRunning", true);
    }

    public void DoubleCoins(bool isdouble)
    {
        isDoubleCoins = isdouble;
    }

    private void Hit()
    {
        MusicController.Instance.HitSound.Play();
        HitPS.Play();
        EZCameraShake.CameraShaker.Instance.ShakeOnce(4f, 10f, .1f, 1f);
    }

    public bool IsOutOfJumps => jumpCount == 0;
    public bool IsShaking => isShaking;
    public bool IsPlayerInvisible => playerInvisible;
}

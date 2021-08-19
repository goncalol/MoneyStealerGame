using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class PoliceController : MonoBehaviour
{
    public Transform[] DestinationPoints;
    public float DistanceRadius = 1f;
    public float viewRadius;
    public float RunSpeed = 7f;
    private float WalkSpeed = 2f;
    private float WalkFast = 3f;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Collider Baton;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    private NavMeshAgent agent;
    private Vector3 currentDestinationTarget;
    private Coroutine routine;
    private Coroutine increaseSpeedRoutine;
    private Coroutine tacklingRoutine;
    private Animator anim;
    private bool IsPatrolling;
    private ExclamationPoint exclamation;
    private ExclamationPoint question;
    private Transform followPlayer;
    private bool IsOnChasing;
    private bool IsOnChasePoint;
    private bool IsTackling;
    private bool IsHittingMode;


    void Start()
    {
        anim = GetComponent<Animator>();
        agent = gameObject.transform.parent.GetComponent<NavMeshAgent>();
        var r = UnityEngine.Random.Range(0, DestinationPoints.Length);
        currentDestinationTarget = DestinationPoints[r].position;
        agent.SetDestination(currentDestinationTarget);
        routine = StartCoroutine("FindTargetsWithDelay", .2f);
        IsPatrolling = true;
    }

    void Update()
    {
        if (IsHittingMode && !PlayerController.Instance.isJumping && Vector3.Distance(transform.parent.position, PlayerController.Instance.transform.position) < 3f) PlayerController.Instance.Die(gameObject.transform);

        if (agent.enabled)
        {
            //if (IsPatrolling && !PlayerController.Instance.IsPlayerInvisible && !PlayerController.Instance.IsWalking && IsOverlapingPlayer()) AwareOfPlayer();//se o player tiver parado(e.g. roubar) e estiver no range to policia

            if (IsPatrolling)
            {
                if (Vector3.Distance(transform.position, currentDestinationTarget) < DistanceRadius)
                {
                    RemoveIfQuestionMark();
                    agent.speed = WalkSpeed;
                    var r = UnityEngine.Random.Range(0, DestinationPoints.Length);
                    currentDestinationTarget = DestinationPoints[r].position;
                    agent.SetDestination(currentDestinationTarget);
                }
                else if (question != null)
                {
                    question.UpdatePosition(transform.parent.GetChild(0).position);
                }
            }
            else if (!IsPatrolling)
            {
                if (PlayerController.Instance.IsPlayerInvisible){
                    ResetPolice(true);
                    return;
                }
                exclamation?.UpdatePosition(transform.parent.GetChild(0).position);
                if (!IsOnChasePoint)
                {
                    if (!IsPlayerInFront())
                    {
                        agent.SetDestination(followPlayer.position);
                        if (Vector3.Distance(transform.parent.position, followPlayer.position) < 1f)
                        {
                            IsOnChasePoint = true;
                            agent.enabled = false;
                            if (tacklingRoutine == null) tacklingRoutine = StartCoroutine("TackleRoutine", 3f);
                            if (increaseSpeedRoutine != null) { StopCoroutine(increaseSpeedRoutine); increaseSpeedRoutine = null; }
                        }
                    }
                    else
                    {
                        agent.SetDestination(PlayerController.Instance.transform.position);
                        if (Vector3.Distance(transform.parent.position, PlayerController.Instance.transform.position) < 7f)
                        {
                            if (tacklingRoutine == null) tacklingRoutine = StartCoroutine("TackleRoutine", 0f);
                            if (increaseSpeedRoutine != null) { StopCoroutine(increaseSpeedRoutine); increaseSpeedRoutine = null; }
                        }
                    }
                }

                LookAtPlayer();
            }
        }
        else if (IsOnChasePoint)
        {
            if (PlayerController.Instance.IsPlayerInvisible)
            {
                ResetPolice(true);
                return;
            }
            transform.parent.position = Vector3.Lerp(transform.parent.position, followPlayer.position, RunSpeed * Time.deltaTime);
            exclamation?.UpdatePosition(transform.parent.GetChild(0).position);

            if (!IsTackling && PlayerController.Instance.IsSlowingDown()) Tackle();

            LookAtPlayer();
        }
    }

    private bool IsPlayerInFront()
    {
        var playerDir = PlayerController.Instance.transform.forward;
        var targetDir = transform.position - PlayerController.Instance.transform.position;
        return Vector3.Angle(targetDir, playerDir) < 45;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (PlayerController.Instance.IsDead) break ;
            if (PlayerController.Instance.IsPlayerInvisible) continue;
            if (FindVisibleTargets())
            {
                AwareOfPlayer();
            }
        }
    }

    IEnumerator IncreaseSpeed(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            agent.speed = agent.speed + 0.1f;
        }
    }

    IEnumerator TackleRoutine(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            if (!IsTackling) Tackle();
        }
    }

    private void Tackle()
    {
        anim.SetTrigger("Tackle");
        IsTackling = true;
    }

    private void AwareOfPlayer()
    {
        IsOnChasing = true;
        StopCoroutine(routine);
        agent.enabled = false;
        anim.SetTrigger("Aware");
        RemoveIfQuestionMark();
        exclamation = NotificationController.Instance.CreateExclamation(transform.parent.GetChild(0).position);
        LookAtPlayer();
        followPlayer = PlayerController.Instance.GetFollowPoint(transform.parent.name);
        Invoke("Chase", .5f);// CancelInvoke("Chase") -> to cancel
        MusicController.Instance.IncreasePitch();
        MusicController.Instance.PoliceAwareSound.Play();
    }

    private void RemoveIfQuestionMark()
    {
        if (question != null)
        {
            Destroy(question.gameObject);
            question = null;
        }
    }

    private void Chase()
    {
        anim.SetTrigger("Run");
        agent.enabled = true;
        agent.speed = RunSpeed;
        IsPatrolling = false;

        if (increaseSpeedRoutine == null)
            increaseSpeedRoutine = StartCoroutine("IncreaseSpeed", 1f);

    }

    private void LookAtPlayer()
    {
        Vector3 targetDirection = PlayerController.Instance.transform.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1, 0.0f);
        transform.parent.rotation = Quaternion.LookRotation(newDirection);
    }

    //https://github.com/SebLague/Field-of-View/blob/master/Episode%2001/Scripts/FieldOfView.cs
    bool FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsOverlapingPlayer()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            if (targetsInViewRadius[i].gameObject.tag == "Player") return true;
            
        }
        return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


    public void EndTackle()
    {
        IsHittingMode = false;
        IsTackling = false;
        IsOnChasing = false;
        if(!PlayerController.Instance.IsDead)
        {
            PlayerController.Instance.UnFollowPoint(transform.parent.name);
            StopCoroutine(tacklingRoutine);
            tacklingRoutine = null;
            agent.enabled = false;
            IsOnChasePoint = false;
            Destroy(exclamation.gameObject);
            Invoke("ResetPatrollRoutineWithFind", 5f);// CancelInvoke("ResetPatrollRoutineWithFind") -> to cancel
            MusicController.Instance.ResetPitch();
        }
        else
        {
            EndedPlayerKO();
        }
    }

    public void ActivateBatonCollider()
    {
        if(!Instructions.Instance.PoliceTackleInstructions())
            IsHittingMode = true;
    }

    public void EndedPlayerKO()
    {
        anim.SetTrigger("KO");
        ResetOtherPolice();
    }

    private void ResetPatrollRoutineWithFind()
    {
        ResetPatrollRoutine();
        routine = StartCoroutine("FindTargetsWithDelay", .2f);
    }

    private void ResetPatrollRoutine()
    {
        anim.SetTrigger("Walk");
        agent.enabled = true;
        agent.speed = WalkSpeed;
        var r = UnityEngine.Random.Range(0, DestinationPoints.Length);
        currentDestinationTarget = DestinationPoints[r].position;
        agent.SetDestination(currentDestinationTarget);
        IsPatrolling = true;
        MusicController.Instance.ResetPitch();
    }

    public void ResetPolice(bool find)
    {
        Destroy(exclamation.gameObject);
        Baton.enabled = false;
        IsTackling = false;
        IsOnChasePoint = false;
        IsOnChasing = false;
        if (tacklingRoutine != null)
        {
            StopCoroutine(tacklingRoutine);
            tacklingRoutine = null;
        }
        if (find) ResetPatrollRoutineWithFind();
        else ResetPatrollRoutine();
        PlayerController.Instance.UnFollowPoint(transform.parent.name);
    }

    public void StopFind()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }

    private void ResetOtherPolice() { 
    
        foreach(var p in GameController.Instance.police)
        {
            if(p.transform.parent.name!=transform.parent.name)
            {
                if (p.IsOnChasing)
                    p.ResetPolice(false);
                else
                    p.StopFind();
            }
        }
    }

    public void CheckingForDisturbs(Vector3 position)
    {
        if (agent.enabled && IsPatrolling)
        {
            if(question==null) question = NotificationController.Instance.CreateQuestion(transform.parent.GetChild(0).position);
            agent.speed = WalkFast;
            currentDestinationTarget = position;
            agent.SetDestination(currentDestinationTarget);
        }
    }


    public void DecreaseRadius()
    {
        viewRadius -= 5;
    }

    public void IncreaseRadius()
    {
        viewRadius += 5;
    }


}

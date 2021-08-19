using UnityEngine;
using UnityEngine.AI;

public class PersonController : MonoBehaviour
{
    private float DistanceRadius = 2f;
    public Collider InteractiveCollider;
    public GameObject ObjectiveInstruction;

    private float PlayerRange = 25f;
    private Animator anim;
    private NavMeshAgent agent;
    private CapsuleCollider personCollider;
    private bool ToDie;
    private Vector3? destinationTarget;
    private PlayerController player;

    void Awake()
    {
        agent = gameObject.transform.parent.GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        player = PlayerController.Instance;
        anim = GetComponent<Animator>();
        personCollider = GetComponent<CapsuleCollider>();
        SetWalking(true);

        if (destinationTarget != null)
        {
            agent.SetDestination(destinationTarget.Value);
        }
        else
        {
            var DestinationPoints = Spawner.Instance.DestinationPoints;
            var randomSpwan = UnityEngine.Random.Range(0, DestinationPoints.Length);
            SetNavigationTarget(DestinationPoints[randomSpwan]);
        }

        if (Instructions.Instance.IsShowingPersonAreaAndTap()) BackAreaInstruction(true);
    }


    void Update()
    {
        if (agent.enabled && Vector3.Distance(transform.position, destinationTarget.Value) < DistanceRadius)
        {
            Spawner.Instance.DestroySpawn(gameObject.transform.parent.gameObject);
        }
    }

    internal void SetWalking(bool walking)
    {
        anim.SetBool("IsWalking", walking);
    }

    public void StartGrabState()
    {
        agent.enabled = false;
        anim.SetBool("isGrabing", true);
        BackAreaInstruction(false);
        SetWalking(false);
    }

    public void ExitGrabState()
    {
        InteractiveCollider.enabled = false;
        personCollider.enabled = false;
        OnStartRunState();
    }

    internal void SetShake(bool shake)
    {
        anim.SetBool("isShaking", shake);
    }

    public void StartTackleState()
    {
        InteractiveCollider.enabled = false;
        personCollider.enabled = false;
        anim.SetTrigger("Tackle");
    }

    internal void OnStartRunState()
    {
        anim.SetBool("IsRunning", true);
        anim.SetBool("isShaking", false);
        anim.SetBool("isGrabing", false);
        agent.enabled = true;
        agent.SetDestination(destinationTarget.Value);
        agent.speed = 8;
        ToDie = true;
    }

    public void SetNavigationTarget(Transform target)
    {
        destinationTarget = target.position;
        if(agent!=null) agent.SetDestination(destinationTarget.Value);
    }

    public void BackAreaInstruction(bool activate) => ObjectiveInstruction.SetActive(activate);
}

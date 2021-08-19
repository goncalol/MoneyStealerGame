using System.Collections;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] PersonPrefab;
    public Grid grid;
    public Transform[] DestinationPoints;
    public int TotalInGameSpawns = 30;

    private PlayerController player;
    private int spawnCount=25;

    private static Spawner _instance;

    public static Spawner Instance { get { return _instance; } }

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
        player = PlayerController.Instance;
        StartCoroutine(waitSeconds(1f));
    }

    IEnumerator waitSeconds(float seconds)
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(seconds);
        }
    }


    private void Spawn()
    {
        if (spawnCount >= TotalInGameSpawns) return;
        //var spwans = grid.GetAdjacentSpawningPoints();
        var randomSpwan = UnityEngine.Random.Range(0, DestinationPoints.Length);
        var randomPerson = UnityEngine.Random.Range(0, PersonPrefab.Length);
        var destination = DestinationPoints[randomSpwan];
        var personParent = Instantiate(PersonPrefab[randomPerson], destination.position, Quaternion.identity);//(GameObject)PrefabUtility.InstantiatePrefab(PersonPrefab);//Resources.Load<GameObject>("PersonParent")
        //personParent.transform.localPosition = SpawnGroup.GetChild(r).localPosition;
        //spawns.Add(personParent);
        spawnCount++;
        var person = personParent.GetComponentInChildren<PersonController>();
        var randomDestination = UnityEngine.Random.Range(0, DestinationPoints.Length - 1);
        person.SetNavigationTarget(DestinationPoints.Except(new[] { destination }).ToArray()[randomDestination].transform);
    }

    public void DestroySpawn(GameObject gameObject)
    {
        spawnCount--;
        Destroy(gameObject);
    }



}

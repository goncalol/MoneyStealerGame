using UnityEngine;

public class EnemyCollider : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other?.gameObject.name == "thieve")
        {
            other.gameObject.GetComponent<PlayerController>().OnCollidedWithEnemy(gameObject);
        }
    }
}

using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        transform.position = new Vector3(player.position.x,20, player.position.z);
    }
}

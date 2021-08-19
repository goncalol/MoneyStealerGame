using UnityEngine;

public class ExclamationPoint : MonoBehaviour
{
    private Camera cam;
    private Vector3 target;
    private float halfWidth;
    private float halfHeight;
    private RectTransform rect;

    void Update()
    {
        if (target != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target);
            if (screenPos.x > Screen.width)
            {
                screenPos.x = Screen.width - halfWidth ;
            }
            else if (screenPos.x < 0)
            {
                screenPos.x = halfWidth ;
            }

            if (screenPos.y > Screen.height)
            {
                screenPos.y = Screen.height - halfHeight ;
            }
            else if (screenPos.y < 0)
            {
                screenPos.y = halfHeight;
            }
            if (screenPos.z < 0)
            {
                screenPos.y = halfHeight;
                screenPos.x = Screen.width - screenPos.x;
            }
            rect.position = screenPos;
        }
    }

    internal void SetParams(Vector3 target, Camera camera)
    {
        cam = camera;
        this.target = target;

        halfWidth = gameObject.GetComponent<RectTransform>().rect.width / 2;
        halfHeight = gameObject.GetComponent<RectTransform>().rect.height / 2;
        rect = GetComponent<RectTransform>();
    }

    public void UpdatePosition(Vector3 newPos)
    {
        target = newPos;
    }
}

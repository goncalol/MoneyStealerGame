using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float YPos;
    private bool swippingUp;
    private float minSwipeDist;

    void Start()
    {
        minSwipeDist = 250 * Screen.height / 2960;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        YPos = eventData.position.y;
        swippingUp = true;
        CancelInvoke("FinishedSwipe");
        Invoke("FinishedSwipe", .5f);
    }

    private void FinishedSwipe()
    {
        swippingUp = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var dist = eventData.position.y - YPos;
        if (eventData.position.y > YPos && swippingUp && Mathf.Abs(dist) > minSwipeDist) Instructions.Instance.Jump();
    }

}

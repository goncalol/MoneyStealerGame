using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsLeft;
    public Button otherButton;
    private PlayerController player;
    private float YPos;
    private bool swippingUp;
    private float minSwipeDist;
    public bool isPointingDown;

    private Coroutine routine;
    private int pointingClicks;

    void Start()
    {
        player = PlayerController.Instance;
        minSwipeDist = 250 * Screen.height / 2960;//adapt 250 -> tested on 2960 pixels height
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player.isIdleStart) { 
            player.JumpStart();
            UIController.Instance.SwitchGameMode();
        }

        isPointingDown = true;
        pointingClicks++;//double tap and hold logic
        if (pointingClicks == 1) routine = StartCoroutine(ResetDoubleTap());
        else if (pointingClicks == 2) StopMoving();

        Turn();
        YPos = eventData.position.y;
        swippingUp = true;
        CancelInvoke("Finished"); 
        Invoke("Finished", .5f);
    }

    IEnumerator ResetDoubleTap()
    {
        yield return new WaitForSeconds(.5f);
        pointingClicks = 0;
    }


    private void Finished()
    {
        swippingUp = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pointingClicks == 2) StartMoving();
        isPointingDown = false;
        if (otherButton.isPointingDown) {
            otherButton.Turn();
            return;
        }
        player.StopTurn();
        var dist = eventData.position.y - YPos;
        if (eventData.position.y > YPos && swippingUp && Mathf.Abs(dist) > minSwipeDist) player.Jump();
    }

    public void Turn()
    {
        player.Turn(IsLeft);
    }

    private void StopMoving()
    {
        if (routine != null) StopCoroutine(routine);
        player.SetMoving(false);
    }

    private void StartMoving()
    {
        player.SetMoving(true);
        pointingClicks = 0;
    }
}

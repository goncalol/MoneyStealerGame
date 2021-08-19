using Assets.Scripts.Powers;
using UnityEngine;

public class Jump : Power
{
    public override void Execute()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        PlayerController.Instance.ResetJumps();
        OnPowerFinished();
    }

    public override void OnPowerFinished()
    {
        GameController.Instance.StartPowerRoutine();
    }
}

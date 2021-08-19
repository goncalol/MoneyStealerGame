using Assets.Scripts;
using Assets.Scripts.Powers;
using UnityEngine;

public class Invisible : Power
{
    public override void Execute()
    {
        timeBonusActive = UIController.Instance.GetPowerUpgradedValue(PowerType.Invisible);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        PlayerController.Instance.SetPlayerInvisible(true);
        StartCoroutine("CountDown");
    }

    public override void OnPowerFinished()
    {
        PlayerController.Instance.SetPlayerInvisible(false);
        GameController.Instance.StartPowerRoutine();
    }
}

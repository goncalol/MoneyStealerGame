using Assets.Scripts;
using Assets.Scripts.Powers;
using UnityEngine;

public class DoubleCoins : Power
{
    public override void Execute()
    {
        timeBonusActive = UIController.Instance.GetPowerUpgradedValue(PowerType.DoubleCoins);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        Clicker.Instance.DoubleCoins(true);
        PlayerController.Instance.DoubleCoins(true);
        StartCoroutine("CountDown");
    }

    public override void OnPowerFinished()
    {
        Clicker.Instance.DoubleCoins(false);
        PlayerController.Instance.DoubleCoins(false);
        GameController.Instance.StartPowerRoutine();
    }
}

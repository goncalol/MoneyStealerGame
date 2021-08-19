using Assets.Scripts;
using Assets.Scripts.Powers;
using UnityEngine;

public class MiniMap : Power
{
    public override void Execute()
    {
        timeBonusActive = UIController.Instance.GetPowerUpgradedValue(PowerType.Map);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        GameController.Instance.minimap.SetActive(true);
        GameController.Instance.minimapCamera.SetActive(true);
        StartCoroutine("CountDown");
    }

    public override void OnPowerFinished()
    {
        GameController.Instance.minimap.SetActive(false);
        GameController.Instance.minimapCamera.SetActive(false);
        GameController.Instance.StartPowerRoutine();
    }
}

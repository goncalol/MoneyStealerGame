using UnityEngine;

public class Shake : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().OnEndShakeState();
    }
}

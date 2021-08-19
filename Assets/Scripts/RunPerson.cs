using UnityEngine;

public class RunPerson : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PersonController>().OnStartRunState();
    }
}

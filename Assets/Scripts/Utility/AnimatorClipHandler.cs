using System.Collections;
using UnityEngine;

// Animator Clip Handler, handles the transitions to another clip if required. Not sure if this works on nodes.
public class AnimatorClipHandler : StateMachineBehaviour
{
    [SerializeField] protected bool crossfade = false;
    [Tooltip("Transition Is Pct - Is this a percentage value (true) or a time value (false)?")]
    [SerializeField] protected bool transitionIsPct = false;
    [Tooltip("Transition Time - Time it takes to make an ending transition. Becomes a percentage of the total length if transitionIsPct = true (Does not apply to the Variations version)")]
    [SerializeField] protected float transitionTime = 0.2f;
    [Tooltip("Next Anim Name - After this state ends,")]
    [SerializeField] protected string nextAnimName = "";

    protected float animTotalLength;
    protected float animCurrentTransitionTime;
    protected int currentAnimNameToHash;
    protected int nameToIndexHash;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animCurrentTransitionTime = animator.GetAnimatorTransitionInfo(0).duration;
        if (nextAnimName != null) nameToIndexHash = Animator.StringToHash(nextAnimName);
        currentAnimNameToHash = stateInfo.fullPathHash;
        float animLength = stateInfo.length + animCurrentTransitionTime;
        animTotalLength = transitionIsPct ? stateInfo.normalizedTime - ((animLength - transitionTime) / animLength) : animLength - transitionTime;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (nextAnimName == "" || animator.GetCurrentAnimatorStateInfo(0).fullPathHash != currentAnimNameToHash) return;

        if (crossfade)
        {
            if (transitionIsPct && (stateInfo.length + transitionTime) > animTotalLength)
            {
                float transitionLength = stateInfo.length * transitionTime;
                animator.CrossFade(nextAnimName, transitionLength, -1, 0, animTotalLength);
            }
            else
            {
                animator.CrossFade(nextAnimName, transitionTime);
            }
        }
        else
        {
            animator.CrossFade(nextAnimName, 0f);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

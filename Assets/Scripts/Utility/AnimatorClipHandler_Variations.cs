using System.Collections;
using UnityEngine;

// Initially inheriting from AnimatorClipHandler, but this is just for setting the random animations to play within a sub-state machine. ADDENDUM: Nevermind, Unity is terrible at telling me I can't play sub-state machines from Play/Crossfade methods.
// Those animations can use the AnimatorClipHandler script to transition out of the sub-state.
public class AnimatorClipHandler_Variations : StateMachineBehaviour
{
    [Tooltip("At what percent should the animation start to transition: 1 - after the whole animation #>1 - before animation finishes")]
    [SerializeField] [Range(0, 1)] protected float transitionStartPct =  1f;
    [SerializeField] protected string[] animNames;
    private string selectedAnim;

    [SerializeField] private bool crossfade = false;
    [Tooltip("Transition Time - Time it takes to make an ending transition. Becomes a percentage of the total length if transitionIsPct = true (Does not apply to the Variations version)")]
    [SerializeField] private float transitionTime = 0.2f;

    private float animCurrentTransitionTime;
    private int nameToIndexHash;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        selectedAnim = animNames[Random.Range(0, animNames.Length)];
        nameToIndexHash = Animator.StringToHash(selectedAnim);
        animCurrentTransitionTime = animator.GetAnimatorTransitionInfo(0).duration;
        //animator.Play(animNames[Random.Range(0, animNames.Length)]);
        if (crossfade)
        {
            animator.CrossFade(nameToIndexHash, transitionTime, -1, 0, transitionStartPct);
        }
        else
        {
            animator.Play(nameToIndexHash);
        }
    }

    //// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        //selectedAnim = animNames[Random.Range(0, animNames.Length)];
        //nameToIndexHash = Animator.StringToHash(selectedAnim);
        //animCurrentTransitionTime = animator.GetAnimatorTransitionInfo(0).duration;
        ////animator.Play(animNames[Random.Range(0, animNames.Length)]);
        //if (crossfade)
        //{
        //    animator.CrossFade(nameToIndexHash, transitionTime, -1, 0, transitionStartPct);
        //}
        //else
        //{
        //    animator.Play(nameToIndexHash);
        //}

        //if (nextAnimName != null)
        //    EndOfAnim();

        //IEnumerable EndOfAnim()
        //{
        //    float animLength = animator.GetCurrentAnimatorStateInfo(0).length + animCurrentTransitionTime;

        //    yield return new WaitForSeconds(animLength);

        //    if (animator.GetCurrentAnimatorStateInfo(0).IsName(selectedAnim)) yield break;

        //    if (!crossfade)
        //    { 
        //        animator.Play(nextAnimName);
        //        yield break;
        //    }

        //    //animTotalLength = transitionIsPct ? animLength 
        //    if (transitionIsPct)
        //    { 
        //        animTotalLength = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - ((animLength - transitionTime) / animLength);
        //        animator.CrossFade(nameToIndexHash, transitionTime * animLength);
        //    }
        //}

    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        //animator.CrossFade(Animator.StringToHash("Idle"), 0.2f);
    }
}

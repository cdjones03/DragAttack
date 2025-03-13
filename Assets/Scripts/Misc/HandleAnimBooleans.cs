using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAnimBooleans : StateMachineBehaviour
{

    public BoolHolder[] boolHolders;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for(int i = 0; i < boolHolders.Length; i++)
        {
            animator.SetBool(boolHolders[i].boolName, boolHolders[i].status);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for(int i = 0; i < boolHolders.Length; i++)
        {
            if(boolHolders[i].resetOnExit)
            {
                animator.SetBool(boolHolders[i].boolName, !boolHolders[i].status);
                
                // If this is a punch animation ending, reset the attack state
                if(boolHolders[i].boolName == "isPunching" || boolHolders[i].boolName == "isKicking")
                {
                    var combatTester = animator.GetComponentInParent<CombatTester>();
                    if(combatTester != null)
                    {
                        combatTester.canAttack = true;
                    }
                }
            } 
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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

    [System.Serializable]
    public class BoolHolder
    {
        public string boolName;
        public bool status;
        public bool resetOnExit;
    }
}

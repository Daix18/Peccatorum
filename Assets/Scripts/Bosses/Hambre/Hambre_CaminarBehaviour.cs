using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hambre_CaminarBehaviour : StateMachineBehaviour
{
    [SerializeField] private float speedMovement;

    private Hambre_Boss_Controller bossHambre;

    private Rigidbody2D rb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossHambre = animator.GetComponent<Hambre_Boss_Controller>();
        rb = bossHambre.rb;
        bossHambre.LookAtPlayer();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Mantener la velocidad en el eje Y igual a la velocidad actual
        float currentYVelocity = rb.velocity.y;

        // Determinar la dirección de movimiento basada en la orientación del jefe
        Vector2 moveDirection = bossHambre.facingRight ? Vector2.right : -Vector2.right;

        // Aplicar la velocidad de movimiento en la dirección adecuada
        rb.velocity = moveDirection * speedMovement + Vector2.up * currentYVelocity;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

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

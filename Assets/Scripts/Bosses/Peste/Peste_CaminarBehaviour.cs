using UnityEngine;

public class Peste_CaminarBehaviour : StateMachineBehaviour
{
    [SerializeField] private float speedMovement;

    private Peste_Boss_Controller bossPeste;

    private Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossPeste = animator.GetComponent<Peste_Boss_Controller>();
        rb = bossPeste.rb;
        bossPeste.LookAtPlayer();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Mantener la velocidad en el eje Y igual a la velocidad actual
        float currentYVelocity = rb.velocity.y;

        // Determinar la direcci�n de movimiento basada en la orientaci�n del jefe
        Vector2 moveDirection = bossPeste.facingRight ? Vector2.right : -Vector2.right;

        // Aplicar la velocidad de movimiento en la direcci�n adecuada
        rb.velocity = moveDirection * speedMovement + Vector2.up * currentYVelocity;
        Debug.Log("Caminar");
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
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
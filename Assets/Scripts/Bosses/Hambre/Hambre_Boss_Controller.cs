using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hambre_Boss_Controller : MonoBehaviour
{
    [Header("Fundamental Components")]
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    public Transform player;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Transform wallChecker;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private Vector3 wallBoxDimensions;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;
    public bool facingRight = true;
    private Vector2 direccion;
    [Header("Vida")]
    [SerializeField] private float life;
    [Header("Attack Settings")]
    [SerializeField] private Transform attackController;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackDamage;
    [Header("Dash Settings")]
    [SerializeField] private bool cooldown = false;    
    [SerializeField] private float dashingPower = 24f;    
    private Vector2 dashingDir;
    private bool isDashing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float distancePlayer = Vector2.Distance(transform.position, player.position);
        animator.SetFloat("playerDistance", distancePlayer);
        animator.SetBool("Cooldown", cooldown);
        animator.SetBool("Dashing", isDashing);

        onGround = Physics2D.OverlapBox(groundChecker.position, dimensionesCaja, 0f, queEsSuelo);

        onWall = Physics2D.OverlapBox(wallChecker.position, wallBoxDimensions, 0f, queEsSuelo);

        if (onGround)
        {
            rb.mass = 100f;
        }
        else
        {
            rb.mass = 1f;
        }

        if (onWall)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;

        if (facingRight)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (!facingRight)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }

        // Invertir la dirección de movimiento
        rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
    }

    public void LookAtPlayer()
    {
        if (player.position.x > transform.position.x && !facingRight || (player.position.x < transform.position.x && facingRight))
        {
            Debug.Log("Flipeo");
            Flip();
        }
    }

    public void Dash()
    {        
        isDashing = true;                
        dashingDir = new Vector2(direccion.x, direccion.y);

        if (dashingDir == Vector2.zero)
        {
            dashingDir = new Vector2(transform.localScale.x, 0);
        }

        if (isDashing)
        {
            // Establecer la velocidad basada en la escala local x del objeto y la potencia de dash
            rb.velocity = dashingDir.normalized * dashingPower;
        }
        StartCoroutine(DashChange());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(attackController.position, attackRadius);
        Gizmos.DrawWireCube(groundChecker.position, dimensionesCaja);
        Gizmos.DrawWireCube(wallChecker.position, wallBoxDimensions);
    }

    IEnumerator DashChange()
    {

        yield return new WaitForSeconds(1f);
    }
}

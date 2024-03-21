using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hambre_Boss_Controller : MonoBehaviour
{
    [Header("Fundamental Components")]
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public bool facingRight = true;
    public Transform player;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Transform wallChecker;    
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private Vector3 wallBoxDimensions;
    [SerializeField] private LayerMask queEsSuelo;    
    [SerializeField] private bool onGround;
    [SerializeField] private bool onWall;    
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
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float cooldownDuration = 4f;
    [SerializeField] private float throwMagnitude = 10f;
    private Vector2 dashingDir;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash;

    [Header("Stun settings")]
    [SerializeField] private bool stun;
    [SerializeField] private int wallHitCount = 0;
    [SerializeField] private int maxWallHits = 3;
    [SerializeField] private float stunDuration = 2f;

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
        animator.SetBool("Stunned", stun);
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

        if (onWall && wallHitCount <= maxWallHits && !stun && !cooldown)
        {            
            rb.velocity *= 0.5f;

            rb.velocity = Vector2.zero;

            wallHitCount++;

            StartCoroutine(WallCollisionSequence());

            if (wallHitCount >= maxWallHits)
            {
                StopCoroutine(WallCollisionSequence());
                StartCoroutine(Stun());
            }
        }

        if (!stun)
        {
            canDash = true;
        }
        else
        {
            canDash = false;
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
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        // Invertir la direcci�n de movimiento
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

        if (canDash)
        {
            dashingDir = new Vector2(direccion.x, direccion.y);
            isDashing = true;        

            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }

            if (isDashing)
            {
                // Establecer la velocidad basada en la escala local x del objeto y la potencia de dash
                rb.velocity = dashingDir.normalized * dashingPower;
            }        
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(attackController.position, attackRadius);
        Gizmos.DrawWireCube(groundChecker.position, dimensionesCaja);
        Gizmos.DrawWireCube(wallChecker.position, wallBoxDimensions);
    }

    //En esta funci�n, hacemos la comprobaci�n de que si hemos chocado contra el player, lo mandamos hacia arriba.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            // Verifica que se encontr� el Rigidbody
            if (playerRb != null)
            {
                // Aplica una fuerza hacia arriba al Rigidbody del jugador
                playerRb.AddForce(Vector2.up * throwMagnitude, ForceMode2D.Impulse);
            }
        }
    }


    IEnumerator WallCollisionSequence()
    {
        Debug.Log("Secuencia");
        Flip();
        yield return new WaitForSeconds(waitTime);

        if (wallHitCount <= maxWallHits)
        {
            Debug.Log("Dash Secuencia");
            canDash = true;
            Dash();
        }
        else if (wallHitCount == maxWallHits)
        {
            canDash = false;
        }
    }

    IEnumerator Stun()
    {
        Debug.Log("Stuneado");
        rb.velocity = Vector2.zero;
        stun = true;
        cooldown = true;
        yield return new WaitForSeconds(stunDuration);
        stun = false;
        wallHitCount = 0;
        yield return new WaitForSeconds(cooldownDuration);
        cooldown = false;
    }
}

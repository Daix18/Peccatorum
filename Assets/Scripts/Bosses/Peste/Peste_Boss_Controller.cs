using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peste_Boss_Controller : MonoBehaviour
{
    [Header("Fundamental Components")]
    private bool facingRight = true;
    public Transform player;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;

    [Header("Vida")]
    [SerializeField] private float life;
    [Header("Attack Settings")]
    [SerializeField] private Transform attackController;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackDamage;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float cooldownTime = 1.5f;
    [SerializeField] private bool cooldown = true;
    [SerializeField] private bool doubleJumped;
    [Header("Gas Ability")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float duration;
    [Header("Stun Settings")]
    [SerializeField] private float stunCooldownTime = 1.5f;
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
        float distancePlayer = Mathf.Abs(transform.position.x - player.position.x);
        animator.SetFloat("playerDistance", distancePlayer);
        animator.SetBool("Cooldown", cooldown);
        animator.SetBool("doubleJumped", doubleJumped);

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }
    private void Death()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        life -= damage;
    }

    public void LookAtPlayer()
    {
        if (player.position.x  > transform.position.x && !facingRight || (player.position.x < transform.position.x && facingRight))  
        {
            facingRight = !facingRight;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }
    }

    public void Jump()
    {
        // Aplicar fuerza al jefe para realizar el double jump
        rb.velocity = Vector2.up * jumpForce;        
    }

    public void SecondJump()
    {
        // Aplicar fuerza al jefe para realizar el double jump
        rb.velocity = Vector2.up * jumpForce;
        StartCoroutine(CooldownChange());
    }

    public void Attack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(attackController.position, attackRadius);

        foreach(Collider2D collision in objects) 
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<AttackController>().TakeDamage(attackDamage);
            }
        }
    }

    public void Down()
    {
        // Detener el movimiento horizontal del jefe
        rb.velocity = new Vector2(0f, rb.velocity.y);

        // Aplicar una fuerza hacia abajo para una caída rápida
        rb.AddForce(Vector2.down * jumpForce * fallMultiplier, ForceMode2D.Impulse);

        doubleJumped = true;
    }

    IEnumerator CooldownChange()
    {
        cooldown = true;       
        yield return new WaitForSeconds(cooldownTime);
        cooldown = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackController.position, attackRadius);
    }
}

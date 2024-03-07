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
    public bool cooldown;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackController.position, attackRadius);
    }
}

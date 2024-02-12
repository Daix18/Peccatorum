using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    private Rigidbody2D rb;
    private TrailRenderer tr;

    public static MovimientoJugador THIS;

    [Header("Movimiento")]
    private float inputX;
    private float movimientoHorizontal = 0f;
    [SerializeField] private float speedMovement;
    [Range(0, 0.3f)][SerializeField] private float suavizadoDeMovimiento;
    private Vector3 velocidad = Vector3.zero;
    private bool mirandoDerecha = true;

    [Header("Salto")]
    [SerializeField] private float jumpingForce;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool onGround;
    private bool jump = false;

    [Header("Wall Slide Settings")]
    [SerializeField] private float wallSlideSpeed;
    private bool onWall;
    private bool wallSliding;

    [Header("Wall Jump Settings")]
    [SerializeField] private float jumpForceWallX;
    [SerializeField] private float jumpForceWallY;
    [SerializeField] private float wallJumpTime;
    private bool wallJumping;

    [Header("SaltoPared")]
    [SerializeField] private Transform wallChecker;
    [SerializeField] private Vector3 wallBoxDimensions;

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    private Vector2 dashingDir;
    private bool canDash = true;
    private bool isDashing;

    [Header("Knife Mechanic Settings")]
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private Transform lanzamientoPosicion;
    [SerializeField] private float fuerzaLanzamiento = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        movimientoHorizontal = inputX * speedMovement;

        if (isDashing)
        {
            rb.velocity = dashingDir.normalized * dashingPower;
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            LanzarCuchillo();
        }

        //Si presionamos el LeftShift y si podemos hacer un dash, realizamos un dash.
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            //Llamamos a la corrutina Dash
            StartCoroutine(Dash());
        }

        if (!onGround && onWall && inputX != 0)
        {
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }
    }

    private void FixedUpdate()
    {
        onGround = Physics2D.OverlapBox(groundChecker.position, dimensionesCaja, 0f, queEsSuelo);

        Move(movimientoHorizontal * Time.fixedDeltaTime, jump);

        onWall = Physics2D.OverlapBox(wallChecker.position, wallBoxDimensions, 0f, queEsSuelo);

        jump = false;

        if (wallSliding && isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    private void Move(float mover, bool jumping)
    {
        if (!wallJumping)
        {
            Vector3 velocidadObjetivo = new Vector2(mover, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);
        }

        if (mover > 0 && !mirandoDerecha)
        {
            Flip();
        }
        else if (mover < 0 && mirandoDerecha)
        {
            Flip();
        }

        if (jumping && onGround && !wallSliding)
        {
            //Salto normal
            Jump();
        }

        //La diferencia en este if es la exclamación
        if (jumping && onWall && wallSliding)
        {
            //Salto en pared
            WallJump();
            Debug.Log(rb.position.y);
        }
    }

    private void Jump()
    {
        onGround = false;
        rb.AddForce(new Vector2(0f, jumpingForce));
    }

    private void WallJump()
    {
        onWall = false;
        rb.velocity = new Vector2(jumpForceWallX * -inputX, jumpForceWallY);
        StartCoroutine(WallJumpChange());
    }

    private void Flip()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void LanzarCuchillo()
    {
        GameObject projectile = Instantiate(knifePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        // Asumiendo que el personaje mira hacia la derecha. Si no, necesitarás ajustar la dirección basándote en la orientación del personaje.
        rb.velocity = new Vector2(transform.localScale.x * fuerzaLanzamiento, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundChecker.position, dimensionesCaja);
        Gizmos.DrawWireCube(wallChecker.position, wallBoxDimensions);
    }

    IEnumerator WallJumpChange()
    {
        wallJumping = true;
        yield return new WaitForSeconds(wallJumpTime);
        wallJumping = false;
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        tr.emitting = true;
        dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dashingDir == Vector2.zero)
        {
            dashingDir = new Vector2(transform.localScale.x, 0);
        }
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
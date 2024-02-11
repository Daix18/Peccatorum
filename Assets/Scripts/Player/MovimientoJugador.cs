using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    private Rigidbody2D rb;

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


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        movimientoHorizontal = inputX * speedMovement;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
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

        if (wallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    public void Move(float mover, bool jumping)
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

    public void Jump()
    {
        onGround = false;
        rb.AddForce(new Vector2(0f, jumpingForce));
    }

    private void WallJump()
    {
        onWall = false;
        rb.velocity = new Vector2(jumpForceWallX * -inputX, jumpForceWallY);
        //rb.AddForce(new Vector2(0f, jumpForceWallY));
        StartCoroutine(WallJumpChange());
    }

    private void Flip()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
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
}
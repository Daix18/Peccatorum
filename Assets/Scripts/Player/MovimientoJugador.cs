using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    private Controls controles;
    private Rigidbody2D rb;
    private TrailRenderer tr;
    [Range(0f, 2f)] [SerializeField] private float timeScale;

    [Header("Movimiento")]
    [SerializeField] private float speedMovement;
    [Range(0, 0.3f)][SerializeField] private float suavizadoDeMovimiento;
    private float inputX;
    private Vector3 velocidad = Vector3.zero;
    private Vector2 direccion;
    private bool mirandoDerecha = true;
    private float _yVelReleaseMod = 2f;
    private float _aumentGravity = 1f;

    [Header("Salto")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int _jumpsLeft;
    [SerializeField] private float jumpingForce;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool onGround;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    private bool jump = false;

    [Header("Wall Slide Settings")]
    [SerializeField] private float wallSlideSpeed;
    private bool onWall;
    private bool wallSliding;

    [Header("Wall Jump Settings")]
    [SerializeField] private int maxDashes = 1;
    [SerializeField] private int _dashesLeft;
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

    [Header("Coyote Time")]
    [Range(0.01f, 0.5f)] [SerializeField] private float coyoteTime;
    [Range(0.01f, 0.5f)] [SerializeField] private float jumpInputBufferTime;
    private float lastOnGroundTime;
    private float lastjumpTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        _jumpsLeft = maxJumps;
        _dashesLeft = maxDashes;
    }

    private void Awake()
    {
        controles = new();
    }

    private void Update()
    {
        Time.timeScale = timeScale;
        direccion = controles.Player.Mover.ReadValue<Vector2>();
        //dash = controles.Playe.Mover.
        inputX = Input.GetAxisRaw("Horizontal");
        bool jumpInputReleased = Input.GetButtonUp("Jump");

        if (isDashing)
        {
            rb.velocity = dashingDir.normalized * dashingPower;
            return;
        }

        if (jumpInputReleased && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y / _yVelReleaseMod);
        }

        if (Input.GetButtonDown("Jump") && _jumpsLeft > 0)
        {
            lastjumpTime = jumpInputBufferTime;
            jump = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (onGround && rb.velocity.y <= 0)
        {
            _jumpsLeft = maxJumps;
            _dashesLeft = maxDashes;
            rb.gravityScale = 1;
        }

        if (rb.velocity.y < 2)
        {
            //Se aumenta la gravedad  cuando caes.
            rb.gravityScale = _aumentGravity * 1.5f;

            //Se limita la velocidad de caída en el eje Y.
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -_maxFallSpeed));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            LanzarCuchillo();
        }

        //Si presionamos el LeftShift y si podemos hacer un dash, realizamos un dash.
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && _dashesLeft > 0)
        {
            //Llamamos a la corrutina Dash
            StartCoroutine(Dash());
        }

        //Salto Buffer
        if (Input.GetButtonDown("Jump") && _jumpsLeft == 0 && jumpBufferCounter > 0)
        {
            Jump(default);
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

        onWall = Physics2D.OverlapBox(wallChecker.position, wallBoxDimensions, 0f, queEsSuelo);

        Move(direccion);

        jump = false;

        if (wallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    private void Move(Vector2 direccion)
    {
        if (!wallJumping)
        {
            Vector3 velocidadObjetivo = new Vector2(direccion.x * speedMovement, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);
        }


        if (onGround && jump)
        {
            lastOnGroundTime = coyoteTime;
        }

        if (direccion.x > 0 && !mirandoDerecha)
        {
            Flip();
        }
        else if (direccion.x < 0 && mirandoDerecha)
        {
            Flip();
        }

        //Comprobación para saltar que incluye el coyote jump.
        if (jump && !wallSliding)
        {
            //Salto normal
            Jump(default);
        }

        //La diferencia en este if es la exclamación
        if (jump && onWall && wallSliding)
        {
            //Salto en pared
            WallJump();
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_jumpsLeft > 0)
        {
            onGround = false;
            rb.AddForce(new Vector2(0f, jumpingForce));
            _jumpsLeft -= 1;
        }
    }

    private void WallJump()
    {
        onWall = false;
        rb.velocity = new Vector2(jumpForceWallX * -inputX, jumpForceWallY);
        jumpBufferCounter = 0f;
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
        GameObject projectile = Instantiate(knifePrefab, lanzamientoPosicion.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        // Asumiendo que el personaje mira hacia la derecha. Si no, necesitarás ajustar la dirección basándote en la orientación del personaje.
        rb.velocity = new Vector2(transform.localScale.x * fuerzaLanzamiento, 0);
    }

    //Cuando entremos en al escena, los controles se cargan
    private void OnEnable()
    {
        controles.Enable();
        controles.Player.Jump.started += Jump;
    }

    //Cuando salgamos de la escena, los controles se desactivan
    private void OnDisable()
    {
        controles.Disable();
        controles.Player.Jump.started -= Jump;
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
        _dashesLeft -= 1;
        dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dashingDir == Vector2.zero)
        {
            dashingDir = new Vector2(transform.localScale.x, 0);
        }
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
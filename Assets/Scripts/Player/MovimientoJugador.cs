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
    //private float inputX;
    //private float inputY;
    private float normalGravity;
    private Vector3 velocidad = Vector3.zero;
    private Vector2 direccion;
    private bool mirandoDerecha = true;

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
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 2f;
    private bool jump = false;

    [Header("Wall Slide Settings")]
    [SerializeField] private float wallSlideSpeed;
    private bool onWall;
    private bool wallSliding;

    [Header("Wall Jump Settings")]
    [SerializeField] private int maxDashes = 1;
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
    [SerializeField] private float dashGravity;
    [SerializeField] private int _dashesLeft;
    private float waitTime;
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
        normalGravity = rb.gravityScale;
    }

    private void Awake()
    {
        controles = new();
    }

    private void Update()
    {
        waitTime += Time.deltaTime;
        Time.timeScale = timeScale;
        direccion = controles.Player.Mover.ReadValue<Vector2>();

        //if (isDashing)
        //{
        //    // Aplicar la velocidad del dash
        //    rb.velocity = dashingDir * dashingPower;
        //    return;
        //}

        //if (jumpInputReleased && rb.velocity.y > 0)
        //{
        //    rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y / _yVelReleaseMod);
        //}

        if (Input.GetButtonDown("Jump") && _jumpsLeft > 0)
        {
            lastjumpTime = jumpInputBufferTime;
            jump = true;
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

        //Si el jugador está cayendo, se multiplica la gravedad y se le resta 1, para que este proporcionada a la gravedad.
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1)  * Time.deltaTime;
        }
        //En el caso de que el jugador está ascendiendo y no se presiona el salto, el salto es más suave.
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            LanzarCuchillo();
        }

        //// Si presionamos el LeftShift y podemos hacer un dash y hay dashes restantes disponibles
        //if (Keyboard.current.leftShiftKey.wasPressedThisFrame && canDash && _dashesLeft > 0)
        //{
        //    // Llamamos a la corrutina Dash
        //    StartDash();
        //    Debug.Log("Velocidad: " + rb.velocity);
        //}

        //Salto Buffer
        if (Input.GetButtonDown("Jump") && _jumpsLeft == 0 && jumpBufferCounter > 0)
        {
            Jump(default);
        }

        if (!onGround && onWall && direccion.x != 0)
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
        if (!wallJumping && !isDashing)
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
            rb.velocity = new Vector2(0f, jumpingForce);
            _jumpsLeft -= 1;
        }
    }

    private void WallJump()
    {
        onWall = false;
        rb.velocity = new Vector2(jumpForceWallX * -direccion.x, jumpForceWallY);
        StartCoroutine(WallJumpChange());
    }
    
    private void Flip()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    public void Dash()
    {
        canDash = false;
        isDashing = true;
        tr.emitting = true;
        rb.gravityScale = dashGravity;
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

        Invoke("StopDash", dashingTime);
    }

    public void StartDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            if (waitTime >= dashingCooldown)
            {
                waitTime = 0;
                Invoke("Dash", 0);  
            }
        }
    }

    public void StopDash()
    {
        canDash = true;
        isDashing = false;
        tr.emitting = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = normalGravity;
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
        controles.Player.Mover.performed += ctx => direccion = ctx.ReadValue<Vector2>(); // Asignar el valor de la dirección del movimiento
        controles.Player.Mover.canceled += ctx => direccion = Vector2.zero; // Limpiar la dirección del movimiento cuando se detiene
    }

    //Cuando salgamos de la escena, los controles se desactivan
    private void OnDisable()
    {
        controles.Disable();
        controles.Player.Mover.performed -= ctx => direccion = ctx.ReadValue<Vector2>(); // Quitar el listener
        controles.Player.Mover.canceled -= ctx => direccion = Vector2.zero; // Quitar el listener
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
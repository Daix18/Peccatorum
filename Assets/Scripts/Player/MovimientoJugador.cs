using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    private Controls controles;
    private Rigidbody2D rb;
    private TrailRenderer tr;
    [Range(0f, 2f)][SerializeField] private float timeScale;

    [Header("Movimiento")]
    [SerializeField] private float speedMovement;
    [SerializeField] private float speedGroundMovement;
    [SerializeField] private float speedAirMovement;

    [Range(0, 0.3f)][SerializeField] private float suavizadoDeMovimiento;
    //private float inputX;
    //private float inputY;
    private float normalGravity;
    private Vector3 velocidad = Vector3.zero;
    private Vector2 direccion;
    private bool mirandoDerecha = true;

    [Header("Salto")]
    [SerializeField] private int _jumpsLeft;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float jumpingForce;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool onGround;
    //private bool jump = false;
    private float jumpBufferCounter;

    [Header("Wall Slide Settings")]
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private bool onWall;
    [SerializeField] private bool wallSliding;

    [Header("Wall Jump Settings")]
    [SerializeField] private float jumpForceWallX;
    [SerializeField] private float jumpForceWallY;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private bool wallJumping;

    [Header("SaltoPared")]
    [SerializeField] private Transform wallChecker;
    [SerializeField] private Vector3 wallBoxDimensions;

    [Header("Dash Settings")]
    [SerializeField] private int maxDashes = 1;
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
    [Range(0.01f, 0.5f)][SerializeField] private float coyoteTime;
    [Range(0.01f, 0.5f)][SerializeField] private float jumpInputBufferTime;
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

        if (onGround && rb.velocity.y <= 0)
        {
            _jumpsLeft = maxJumps;
            _dashesLeft = maxDashes;
            //rb.gravityScale = 1;
            //jump = false;
        }

        //Si el jugador está cayendo, se multiplica la gravedad y se le resta 1, para que este proporcionada a la gravedad.
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        //En el caso de que el jugador está ascendiendo y no se presiona el salto, el salto es más suave.
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }


        ////Salto Buffer
        //if (Input.GetButtonDown("Jump") && _jumpsLeft == 0 && jumpBufferCounter > 0)
        //{
        //    Jump();
        //}

        if (!onGround && onWall && direccion.x != 0)
        {
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }

        //Si no ha hecho un wall jump, está pegado en una pared y está haciendo un wall Slide, se hace un wall Jump.
        if (!wallJumping && onWall && wallSliding)
        {
            if (Input.GetButtonDown("Jump"))
            {
                //Salto en pared
                WallJump();
            }
        }
    }

    private void FixedUpdate()
    {
        onGround = Physics2D.OverlapBox(groundChecker.position, dimensionesCaja, 0f, queEsSuelo);

        onWall = Physics2D.OverlapBox(wallChecker.position, wallBoxDimensions, 0f, queEsSuelo);

        if (onGround)
            speedMovement = speedGroundMovement;
        else
            speedMovement = speedAirMovement;

        //Se aplica el moviemiento del jugador, en terminos de velocidad.
        if (!wallJumping && !isDashing)
        {
            Vector3 velocidadObjetivo = new Vector2(direccion.x * speedMovement, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);
        }

        if (!wallJumping && wallSliding)
        {
            wallJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    private void Move()
    {
        direccion = controles.Player.Mover.ReadValue<Vector2>();

        //if (onGround && jump)
        //{
        //    lastOnGroundTime = coyoteTime;
        //}

        if (direccion.x > 0 && !mirandoDerecha)
        {
            Flip();
        }
        else if (direccion.x < 0 && mirandoDerecha)
        {
            Flip();
        }
    }

    private void Jump()
    {
        if (_jumpsLeft > 0)
        {
            onGround = false;
            rb.velocity = new Vector2(0f, jumpingForce);
            _jumpsLeft -= 1;
            //jump = true;
            Debug.Log("Salto");
        }
    }
    private void Flip()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void WallJump()
    {
        onWall = false;
        rb.velocity = new Vector2(-direccion.x * jumpForceWallX, jumpForceWallY);
        Debug.Log("Wall Jump");
        StartCoroutine(WallJumpChange());
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
    void LanzarCuchillo()
    {
        GameObject projectile = Instantiate(knifePrefab, lanzamientoPosicion.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        // Asumiendo que el personaje mira hacia la derecha. Si no, necesitarás ajustar la dirección basándote en la orientación del personaje.
        rb.velocity = new Vector2(transform.localScale.x * fuerzaLanzamiento, 0);
    }

    //Estas funciones son llamadas desde el componente de player input, el cual lo contiene el player.
    //Para verlas le damos a evet
    public void StartMove(InputAction.CallbackContext context)
    {
        Move();
    }

    public void StartJump(InputAction.CallbackContext context)
    {
        //Comprobación para saltar que incluye el coyote jump.
        if (context.performed && !wallSliding)
        {
            //Salto normal
            Jump();
        }
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

    public void StartKnife(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LanzarCuchillo();
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

    //Corrutinas:

    //Corrutina del cambio de WallJump
    IEnumerator WallJumpChange()
    {
        wallJumping = true;
        yield return new WaitForSeconds(wallJumpTime);
        wallJumping = false;
        //canMoveSideways = true;
    }
}
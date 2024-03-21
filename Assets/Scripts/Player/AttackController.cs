using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float health;
    [SerializeField] private float radioGolpe;
    [SerializeField] private float dañoGolpe;
    [SerializeField] private float tiempoEntreAtaques;
    [SerializeField] private float tiempoSiguienteAtaque;
    private Animator animator;
    [SerializeField] private float initialHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = initialHealth;
    }

    private void Update()
    {
        if (tiempoSiguienteAtaque > 0)
        {
            tiempoSiguienteAtaque -= Time.deltaTime;
            
        }

        if (Input.GetButtonDown("Fire1") && tiempoSiguienteAtaque <= 0)
        {
            Golpe();
            tiempoSiguienteAtaque = tiempoEntreAtaques;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Asegurarse de que la vida no sea negativa
            RespawnPlayer();
        }
    }
    private void RespawnPlayer()
    {
        RespawnSystem respawnSystem = GameObject.FindGameObjectWithTag("Respawn").GetComponent<RespawnSystem>();
        if (respawnSystem != null)
        {
            transform.position = respawnSystem.GetLastSpawnPoint();
            currentHealth = initialHealth;
        }
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    private void Golpe()
    {
        //animator.SetTrigger("Golpe");

        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemigo"))
            {
                colisionador.transform.GetComponent<Enemigo>().TomarDaño(dañoGolpe);
            }
        }
    }
    public void ResetHealth()
    {
        currentHealth = initialHealth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }
}

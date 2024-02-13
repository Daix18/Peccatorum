using System.Collections;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Busca al jugador en la escena y obtiene su transform
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Diana"))
        {
            // Si colisiona con una "Diana", teletransporta al jugador
            StartCoroutine(TeleportPlayer());
        }
    }

    IEnumerator TeleportPlayer()
    {
        // Detener el movimiento del cuchillo
        rb.velocity = Vector2.zero;
        // Desactivar el Rigidbody del cuchillo
        rb.simulated = false;

        // Esperar un breve momento antes de teletransportar al jugador
        yield return new WaitForSeconds(0.5f);

        // Teletransportar al jugador a la posición del cuchillo
        player.position = transform.position;
    }
}

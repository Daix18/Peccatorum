using Ink.Parsed;
using System.Collections;
using UnityEngine;

public class KnfieController : MonoBehaviour
{
    //[SerializeField] private Transform teleportPosition;

    private Transform player;

    void Start()
    {
        // Busca al jugador en la escena y obtiene su transform
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Diana"))
        {
            // Si colisiona con una "Diana", teletransporta al jugador
            if (player != null)
            {
                player.position = transform.position;
                // Opcional: Añade aquí una animación o efecto para la teletransportación
            }
        }
        // Destruye el proyectil en cualquier caso
        Destroy(gameObject);
    }
}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Suelo"))
    //    {
    //        // Si choca con una pared, destruir el cuchillo
    //        Destroy(gameObject);
    //    }        
    //}

    //IEnumerator TeleportPlayer(Vector2 teleportPosition)
    //{
    //    rb.velocity = Vector2.zero;
    //    rb.simulated = false; // Desactivar el Rigidbody2D.

    //    yield return new WaitForSeconds(0.5f);

    //    // Teletransportar al jugador a la posición de la diana
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");
    //    player.transform.position = teleportPosition;
    //    // Destruir el cuchillo
    //    //Destroy(gameObject);
    //}

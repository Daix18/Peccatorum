using Ink.Parsed;
using System.Collections;
using UnityEngine;

public class KnfieController : MonoBehaviour
{
    [SerializeField] private Transform teleportPosition;

    public Transform player;

    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Diana")) // Asegúrate de que tu tabla de madera tenga este tag
        {
            player.position = transform.position; // Teletransporta al jugador a la posición del proyectil
            // Opcional: Añade aquí una animación o efecto
            Destroy(gameObject); // Destruye el proyectil
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
}

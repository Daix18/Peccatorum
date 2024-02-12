using UnityEngine;

public class KnfieController : MonoBehaviour
{
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Diana"))
        {
            // Teletransportar al personaje principal a la posición de la colisión
            GameObject personaje = GameObject.FindGameObjectWithTag("Player");
            personaje.transform.position = transform.position; // O utiliza other.transform.position si lo prefieres
            // Detener el movimiento del objeto actual
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
        }
    }

}

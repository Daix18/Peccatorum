using UnityEngine;

public class ThrowingKnifeController : MonoBehaviour
{
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private Transform lanzamientoPosicion;
    [SerializeField] private float fuerzaLanzamiento = 10f;

    private GameObject cuchilloLanzado;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && cuchilloLanzado == null)
        {
            LanzarCuchillo();
        }
    }

    void LanzarCuchillo()
    {
        cuchilloLanzado = Instantiate(knifePrefab, lanzamientoPosicion.position, Quaternion.identity);
        Rigidbody2D rb = cuchilloLanzado.GetComponent<Rigidbody2D>();

        // Determinar la dirección del lanzamiento según la dirección del personaje principal
        Vector2 direccionLanzamiento = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        rb.velocity = direccionLanzamiento * fuerzaLanzamiento;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Diana"))
        {
            // Teletransportar al personaje principal a la posición de la colisión
            GameObject personaje = GameObject.FindGameObjectWithTag("Player");
            personaje.transform.position = other.transform.position;

            // Destruir el cuchillo
            Destroy(cuchilloLanzado);
            cuchilloLanzado = null;
        }
        else if (other.gameObject.CompareTag("Pared"))
        {
            // Si choca con una pared, destruir el cuchillo sin teletransportar al jugador
            Destroy(cuchilloLanzado);
            cuchilloLanzado = null;
        }
    }
}

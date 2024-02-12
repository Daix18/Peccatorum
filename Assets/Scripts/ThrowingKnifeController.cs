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
}

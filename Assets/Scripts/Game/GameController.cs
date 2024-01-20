using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Image Components.")]
    [SerializeField] private Image mapa;
    [SerializeField] private Image playerIcon;

    [SerializeField] private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            //Se pausa el tiempo y se muestra el mapa.
            Time.timeScale = 0f;
            mapa.enabled = true;
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            //Se reanuda el tiempo y se desactiva el mapa.
            Time.timeScale = 1f;
            mapa.enabled = false;
        }
    }
}

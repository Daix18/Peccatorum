using UnityEngine;

public class NPController : MonoBehaviour
{
    public static NPController THIS;

    public bool playerIsClose;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        THIS = this;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
        }
    }
}

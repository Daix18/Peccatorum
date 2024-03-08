using UnityEngine;

public class GasController : MonoBehaviour
{
    [SerializeField] private Transform attackController;
    [SerializeField] private float gasDamage;
    [SerializeField] private float gasRadius;
    [SerializeField] private float gasDuration;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(attackController.position, gasRadius);

        foreach (Collider2D collision in objects)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("Se ha restado vida");
                collision.GetComponent<AttackController>().TakeDamage(gasDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackController.position, gasRadius);
    }
}

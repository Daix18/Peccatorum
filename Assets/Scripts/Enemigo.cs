using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [SerializeField] private float vida;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void TomarDaño(float daño)
    {
        vida -= daño;

        if (vida <= 0)
        {
            Muerte();
        }
    }
    private void Muerte()
    {
        //animator.SetTrigger("Muerte");
    }
}

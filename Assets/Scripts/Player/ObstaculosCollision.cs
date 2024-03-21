using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculosCollision : MonoBehaviour
{
    private const float damageAmount = 200f; // Cantidad de daño que recibe el jugador al colisionar
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<AttackController>().TakeDamage(damageAmount);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    private Vector3 lastSpawnPoint; // Variable para almacenar la posición del último "spawn point"

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Actualizar la posición del último "spawn point" solo si la vida del jugador es menor o igual a cero
            AttackController attackController = other.GetComponent<AttackController>();
            if (attackController != null && attackController.GetCurrentHealth() <= 0)
            {
                lastSpawnPoint = transform.position;
            }
        }
    }

    public Vector3 GetLastSpawnPoint()
    {
        return lastSpawnPoint;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    private Vector2 lastSpawnPoint; // Variable para almacenar la posición del último "spawn point"

    private void Start()
    {
        lastSpawnPoint = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Actualizar la posición del último "spawn point" al pasar por el spawn point
            Debug.Log("Player ha pasado por el spawn point: " + transform.position);
            lastSpawnPoint = transform.position;
        }
    }

    public Vector2 GetLastSpawnPoint()
    {
        return lastSpawnPoint;
    }   
}

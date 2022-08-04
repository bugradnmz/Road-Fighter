using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireSpawner : MonoBehaviour
{
    [SerializeField] GameObject tire;
    GameObject player;

    [SerializeField] float spawnDistance; //min distance of player to spawn
    float distance; //Distance of player
    bool isSpawned;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        distance = transform.position.z - player.transform.position.z;

        if (distance <= spawnDistance && !isSpawned)
        {
            SpawnTire();
            Destroy(gameObject); //Optimization purposes. Spawn other and destroy itself
        }
    }

    void SpawnTire()
    {
        Instantiate(tire, transform.position, Quaternion.identity);
        isSpawned = true;
    }
}

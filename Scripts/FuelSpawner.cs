using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelSpawner : MonoBehaviour
{
    [SerializeField] GameObject fuel;
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
            SpawnFuel();
            Destroy(gameObject); //Optimization purposes. Spawn other and destroy itself
        }
    }

    void SpawnFuel()
    {
        Instantiate(fuel, transform.position, Quaternion.identity);
        isSpawned = true;
    }
}

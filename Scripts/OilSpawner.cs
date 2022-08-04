using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpawner : MonoBehaviour
{
    [SerializeField] GameObject oil;
    GameObject player;

    [SerializeField] float spawnDistance; //min distance of player to spawn oil
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
            SpawnOil();
            Destroy(gameObject); //Optimization purposes. Spawn other and destroy itself
        }
    }

    void SpawnOil()
    {
        Instantiate(oil,transform.position - new Vector3 (3.15f, 0 ,0),Quaternion.identity);
        isSpawned = true;
    }
}

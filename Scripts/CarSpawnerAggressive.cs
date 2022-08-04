using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnerAggressive : MonoBehaviour
{
    [SerializeField] GameObject[] car;
    GameObject player;

    [SerializeField] float spawnDistance; //min distance of player to spawn car
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
            SpawnAggressiveCar();
            Destroy(gameObject); //Optimization purposes. Use and destroy
        }
    }

    void SpawnAggressiveCar()
    {
        int carNumber = Random.Range(0, car.Length);
        GameObject AI = Instantiate(car[carNumber], transform.position, Quaternion.Euler(0, -90, 0));
        AI.GetComponent<AI>().type = "aggressive";
        isSpawned = true;
    }
}

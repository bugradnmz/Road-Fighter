using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour //Destroys game objects after we pass them for optimization purposes.
{
    GameObject player;

    [SerializeField] float destroyDistance; //min distance of player to destroy
    float distance; //Distance of player

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        distance = player.transform.position.z - transform.position.z;

        if (distance >= destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRoad : MonoBehaviour  //
{
    [SerializeField] GameObject[] road;
    [SerializeField] GameObject mountains;
    [SerializeField] GameObject player;
    
    void Start()
    {
        
    }

    void Update()
    {
        for (int i = 0; i < road.Length; i++)
        {
            if (player.transform.position.z - 12 > road[i].transform.position.z)
            {
                road[i].transform.position += new Vector3(0, 0, 640);
            }
        }
        mountains.transform.position = player.transform.position + new Vector3(0, 0, 65);
    }
}

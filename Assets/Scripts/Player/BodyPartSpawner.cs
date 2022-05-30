using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartSpawner : MonoBehaviour
{
    [SerializeField] GameObject collectableToSpawn;
    

     public void SpawnBodyCollectable()
    {
        Instantiate(collectableToSpawn, transform.position, Quaternion.identity);
    }


}

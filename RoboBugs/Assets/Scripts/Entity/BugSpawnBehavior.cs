using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugSpawnBehavior : MonoBehaviour
{
    [SerializeField] GameObject bugToSpawn;
    [SerializeField] float spawnRate = 5.0f;
    float spawnTimer = 0.0f;
    [SerializeField] Transform spawnPointTransform;

    // Update is called once per frame
    void Update()
    {
        if(spawnTimer >= spawnRate)
        {
            //spawn a bug at spawnPointTransform
            Instantiate(bugToSpawn, spawnPointTransform.position, Quaternion.identity);

            //reset spawnTimer
            spawnTimer = 0.0f;
        }
        else
        {
            //increment spawnTimer
            spawnTimer += Time.deltaTime;
        }
    }
}

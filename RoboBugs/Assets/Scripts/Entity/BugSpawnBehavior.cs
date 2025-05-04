using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugSpawnBehavior : MonoBehaviour
{
    [SerializeField] GameObject bugToSpawn;
    [SerializeField] float spawnRate = 5.0f;
    float spawnTimer = 0.0f;
    [SerializeField] Transform spawnPointTransform;
    [SerializeField] int maxSpawnCount = 3;

    // Update is called once per frame
    void Update()
    {
        //as long as the nest has NOT already spawned enough bugs,...
        if (gameObject.transform.childCount - 2 < maxSpawnCount)
        {
            if (spawnTimer >= spawnRate)
            {
                //spawn a bug at spawnPointTransform and set its parent to this spawner
                Instantiate(bugToSpawn, spawnPointTransform.position, Quaternion.identity, gameObject.transform);

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
}

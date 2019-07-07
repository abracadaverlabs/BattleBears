using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int team;
    public AIController aiPrefab;

    public float spawnInterval;

    //TODO Finish this script to spawn a new ai every 3 seconds

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    //This calls the Spawn function every 'spawnInterval' seconds
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Spawn();
        }
    }

    void Spawn ()
    {
        AIController aiClone = Instantiate(aiPrefab, transform.position, transform.rotation);
        aiClone.team = team;
    }
}

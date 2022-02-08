using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int number;
    public float spawnRadius;
    // Start is called before the first frame update
    void Start()
    {
        //random spawn of the zombies
        for (int i = 0; i < number; i++)
        {
            Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * spawnRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
                Instantiate(zombiePrefab, hit.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

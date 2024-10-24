using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class block_spawnUP_tutorial : MonoBehaviour
{
    public GameObject block;
    int secondsForSpawn = 20;
    int spawnOffset = 18;
    bool keep_going = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(blockSpawnUp());
    }

    public void stopBlocks()
    {
        keep_going = false;
    }

    IEnumerator blockSpawnUp()
    {
        yield return new WaitForSeconds(spawnOffset);

        while (keep_going == true)
        {
            Instantiate(block, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(secondsForSpawn);
        }
    }
}

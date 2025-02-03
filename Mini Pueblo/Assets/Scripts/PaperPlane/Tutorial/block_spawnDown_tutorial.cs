using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class block_spawnDown_tutorial : MonoBehaviour
{
    public GameObject block;
    public GameObject hand;
    int secondsForSpawn = 16;
    int initialOffset = 6;
    int spawnOffset = 4;
    bool keep_going = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(blockSpawnDown());
    }

    public void stopBlocks()
    {
        keep_going = false;
    }

    IEnumerator blockSpawnDown()
    {
        yield return new WaitForSeconds(initialOffset);
        TextTutorial.instance.nextText();
        hand.SetActive(true);
        yield return new WaitForSeconds(spawnOffset);

        while (keep_going == true)
        {
            Instantiate(block, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(secondsForSpawn);
        }
    }
}

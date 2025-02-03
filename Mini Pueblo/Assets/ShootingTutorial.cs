using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTutorial : MonoBehaviour
{
    bool pressed = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pressed = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            pressed = false;
        }

        if (pressed == false)
        {
            TextTutorial.instance.goToIndex(0);
        }
        else
        {
            TextTutorial.instance.goToIndex(1);
        }
    }
}

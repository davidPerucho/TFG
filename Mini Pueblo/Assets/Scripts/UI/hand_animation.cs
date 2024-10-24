using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand_animation : MonoBehaviour
{
    Animator animator;
    bool pass = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void change_animation()
    {
        if (pass == true)
        {
            pass = false;
        }
        else
        {
            pass = true;
        }

        animator.SetBool("Pass", pass);
    }
}

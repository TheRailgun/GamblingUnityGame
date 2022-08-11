using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script makes sure through menuing we don't accidentally make multiple music players.
 *
 * This script example manages the playing audio. 
 * The GameObject with the "music" tag is the BackgroundMusic GameObject. 
 * The AudioSource has the audio attached to the AudioClip.
 */

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("music");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
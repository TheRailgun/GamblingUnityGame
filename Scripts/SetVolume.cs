using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/* Simple script to change the volume of the game using the slider's value.
 * 
 * The main highlight is the Mathf.Log10 * 20 which makes the slider value more accurately reflect volume because mixer fader ranges are logarithmic whereas sliders are linear
 */

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    
    public void SetLevel(float sliderValue){
        mixer.SetFloat("MusicVol",Mathf.Log10(sliderValue)*20);//Makes 1% of the audio slider into 1% volume.

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* The purpose of this script its to manipulate the values that display the values won, then tween into the "Last Win" or "Current Balance" displays.
 * The "SmallWin" fuctions are called for the "Last Win" value because that updates for every chest opened with monetary gain.
 * 
 * An instance of winnings.cs is intended to be added to whichever text you wish you to display, tween, then hide and reset position. 
 */

public class winnings : MonoBehaviour
{
    private Vector3 spawnPos;
    public GameObject gameMaster;
    float runtime = 1.2f;//how long the animation of the tween will run for.
    void Start()
    {
        spawnPos = this.gameObject.transform.localPosition;
    }

/* Functions displayWin() and displaySmallWin() are functionally the same, they just take different input types and display them in text, tween the text to the desired location, then call a function to hide themselves.
 * Method overload would be an alternative approach. 
 *
 * The main difference between the two functions are displayWin() calls a game reset (because I want the animation to finish before a game reset occurs) and displaySmallWin just calls an update on the LastWin display.
 */
    public void displayWin(float win){
        this.GetComponent<UnityEngine.UI.Text>().enabled = true;
        this.GetComponent<UnityEngine.UI.Text>().text = ("$ " + win.ToString("F2"));//Makes float into money format.
        LeanTween.moveLocal(this.gameObject, new Vector3(spawnPos.x,-30,0),runtime).setEase(LeanTweenType.easeOutBounce);
        Invoke("hideWin", (runtime));
    }
    
    public void displaySmallWin(string win){
        this.GetComponent<UnityEngine.UI.Text>().enabled = true;
        this.GetComponent<UnityEngine.UI.Text>().text = win;
        LeanTween.moveLocal(this.gameObject, new Vector3(spawnPos.x,-30,0),runtime).setEase(LeanTweenType.easeOutBounce);
        Invoke("hideSmallWin", (runtime));
    }

/* Both the next functions hide the text, return it to its original position, and call a function from the gameMaster script. 
 *
 * updateLastWin() updates that text's display and resetGame reset's the game state.
 */
    void hideSmallWin(){
        gameMaster.GetComponent<Game>().updateLastWin();
        this.GetComponent<UnityEngine.UI.Text>().enabled=false;
        this.gameObject.transform.localPosition = spawnPos;
    }

    void hideWin(){
        gameMaster.GetComponent<Game>().resetGame();
        this.GetComponent<UnityEngine.UI.Text>().enabled=false;
        this.gameObject.transform.localPosition = spawnPos;
    }
}

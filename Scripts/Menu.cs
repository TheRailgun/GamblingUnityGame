using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Script to load game/quit game on the main menu. Options is handled entirely in Unity buttons.
 *
 */

public class Menu : MonoBehaviour
{
    public void StartGame(){
        SceneManager.LoadScene("Game");
    }

    public void QuitGame(){
        Application.Quit();
    }
}

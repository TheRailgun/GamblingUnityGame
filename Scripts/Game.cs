using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This scripts contains the logic for the main game along with some functions used externally to either edit the gamestate or to get information on the gamestate. 
 * Many elements will have an in Unity pointer to this script to reference it and this script will have public points to many objects. This is discussed more in the documentation.
 * 
 * 
 * 
 */



public class Game : MonoBehaviour
{
    public bool playing = false;
    public Text currBalanceText;
    public Text currBetText;
    public Text lastWinText;
    public Button playB;
    public Button increaseB;
    public Button decreaseB;
    public Text totalWins;
    public Text smallWins;
    public GameObject[] chests;
    public AudioSource audiosource;
    public AudioClip success;
    public AudioClip failure;
    
    float balance = 10.00f;
    float bet = 0.25f;
    float win = 0;
    float[] results = {0,0,0,0,0,0,0,0,0};
    int resultsPointer = 1;
    float betType = 0;



/* In the start function we update balance, current bet, and last win. These SHOULD be redundant however can never be too safe incase the Unity values are changed in testing.
 * We also fill the chests array will all chests in the scene. This is done differently than the static pointers because chests are a prefab. Allows for adding/removing chest elements easily(though some code would need to be edited). 
 *
 *
 */
    void Start()
    {
        updateBalance(0);
        currBetText.text = "Current Bet: $"+ bet.ToString("F2");//this sets the "bet" float value into $ X.XX format. This will appear fequently in the project.
        updateLastWin();
        chests = GameObject.FindGameObjectsWithTag("Chest");
    }

/* Method to update the balance text. Takes current balance and adds the change. Change can be negative for when play begins and money is used in the bet.
 * Is not public as it is only used in this scope.
 *
 */
    void updateBalance(float change){
        balance += change;
        currBalanceText.text = "Current Balance: $"+ balance.ToString("F2");//formats in X.XX format
    }

/* Method to update the "last win" text. This is public so that it can be called from "winnings.cs" when a chest is opened to update the last win as chests are opened. 
 *
 */
    public void updateLastWin(){
        lastWinText.text = "Last Win $"+ win.ToString("F2");//formats in X.XX format
    }


/* Function called whenever "Decrease" button is hit. Decreases the current bet based on the denomination tier's laid out in the project specifics.
 * Also updates "bet" text to reflect this change to the player. Lastly disable's the play button if the player doesn't have the required funds.
 *
 */
    void decreaseBet(){
        betType=betType-1;
        if(betType < 1){
            betType = 0;//in the event you try to decrease at the lowest tier this resets pointer to the lowest tier.
            bet = 0.25f;
        } else if (betType == 1){
            bet = 0.50f;
        } else if (betType == 2){
            bet = 1.00f;
        } //dont need the 4th option cause you're never going to decrease to the highest option.
        currBetText.text = "Current Bet: $"+ bet.ToString("F2");
        if(bet>balance){disablePlay();}else{enablePlay();}//disables the play button if the player does not have the required funds. Otherwise enables play to swap back.
    }


/* Inverse of the decreaseBet() function. Increases the bet denomination by the tier's laid out in the project assignment.
 * 
 */
    void increaseBet(){
        betType=betType+1;
        if (betType == 1){
            bet = 0.50f;
        } else if (betType == 2){
            bet = 1.00f;
        } else if (betType > 2){
            bet = 5.00f;
            betType = 3;
        }
        currBetText.text = "Current Bet: $"+ bet.ToString("F2");
        if(bet>balance){disablePlay();}//does not need a enable play case because you will never increase into an amount that enables play where it was previously disabled.
    }

/* Public function which returns the state of the "playing" bool. Primarily used to inform game objects about their interactability
 *
 */
    public bool getPlaying(){
        return playing;
    }

/* Two functions to disable and enable the play button. Using playB.interactable changes the button color to the "grayed out" disabled color per assignment
 * Two functions to disable and enable the play button AND the increase/decrease buttons for when the game is playing. 
 *
 */
    void disablePlay(){
        playB.interactable = false;
    }

    void enablePlay(){
        playB.interactable = true;
    }

    void disableAll(){
        disablePlay();
        increaseB.interactable = false;
        decreaseB.interactable = false;
    }

    void enableAll(){
        enablePlay();
        increaseB.interactable = true;
        decreaseB.interactable = true;
    }


/* Public function to call when chest's need to display a value when opened. Fetches monetary values from the "results" array using "resultsPointer" to indicate which value is next to be delivered.
 * Plays an audio using the audio mixer established in the main menu (therefore retaining volume preferences) to play a sound on victory or defeat.
 * Returns a string to the chests but could easily have returned a float that could be interpreted in the chest code instead.
 * As this function is more complicated, there will be inline comments to help with understanding. 
 *
 */
    public string getValue(){
        if(results[resultsPointer]!=0){ //checks to see if there are prizes left to give. If the game is an auto loss this will be 0
            audiosource.PlayOneShot(success,1f); 
            smallWins.GetComponent<winnings>().displaySmallWin("$ " + results[resultsPointer].ToString("F2")); //calls function to set floating green text to lastwin value and update that
            win = win + results[resultsPointer];//updates internal win tracker
            resultsPointer--;//moves through array to the next prize
            return "$ " + results[resultsPointer+1].ToString("F2");//returns winning as string to be displayed
        } else {
            audiosource.PlayOneShot(failure,1f);
            playing = false; //set playing to false before stopPlaying() so that no more chests can be clicked.
            Invoke("stopPlaying", 1); //delayed invoke to allow for animations etc.
            return "Bust!";
        }
    }


/* Generates the result interms of 1x 2x ... 500x return on player investment. Follows the odds described in the assignment and uses some truncation tricks to minimize math. 
 * Called when play starts to predetermine how much players will receive in total. Then calls "populateArray()" to split up this total win # into different chests.
 * Debug.Log's intentionally left here so testers in unity can easily see expected result.
 *
 * As this function is more complicated, there will be inline comments to help with understanding.
 */
    void generateResults(){
        int chance  = Random.Range(1,101);//The required odds are based on % chance so this is a number 1-100 to represent that %
        if(chance < 51){ //bottom 50% is instant loss
            resultsPointer = 0;
            results[0] = 0;
            Debug.Log(0);
        } else if(chance < 81){// next 30% is 1x 2x ..... 10x
            chance = chance - 50;
            chance = (chance+2)/3;//expected value is 1-30. Results should be: 1-3 = 1 4-6 = 2 7-9 = 3 10-12 = 4 13-15 = 5 16-18 = 6 19-21 = 7 22-24 = 8 25-27 = 9 28-30 = 10|division rounds down therefore by adding 2 to the value we start with we get the desired results
            Debug.Log(chance);
            populateArray(chance*bet);
        } else if(chance <96){//12x 16x 24x 32x 48x 64x | 6 results across 15%. 15x2 => 30 + 4 => 34 then we will divide by 5 like before to get desired results.
            chance = chance - 80;
            chance = (chance*2+4)/5;
            if(chance == 1){populateArray(12*bet);Debug.Log(12);}
            else if(chance == 2){populateArray(16*bet);Debug.Log(16);}
            else if(chance == 3){populateArray(24*bet);Debug.Log(24);}
            else if(chance == 4){populateArray(32*bet);Debug.Log(32);}
            else if(chance == 5){populateArray(48*bet);Debug.Log(48);}
            else if(chance == 6){populateArray(64*bet);Debug.Log(64);}
        } else if(chance <101){//100x-500x. 
            chance = chance - 95;//Chance is either 96, 97, 98, 99, or 100 so removing 95 leaves us with 1-5
            Debug.Log(chance*100);
            populateArray(chance*100*bet);
        }
    }
/* Function called by generate results when the player wins and needs to have winnings divided between chests.
 * If the winnings are low (sub 5$), players winnings can be split at max 5 ways. This is to prevent values that dont end in .05 (worse case being 1x 0.25/5 = 0.05 splits)
 * Minor side effect is players receive more chests when betting & winning more money. This could increase player enjoyment at higher bet levels and subconciously encourage it.
 * If the winnings are higher than 5$, players winnings can be split and placed in chests with X/50 parts in each chest. Worst case being 1x 5.00/50 = 0.1 splits.
 *
 * As this function is more complicated, there will be inline comments to help with understanding.
 */
    void populateArray(float total){
        int split = 50;
        if(total <= 5){//checks if winnings are small enough to reduce reward splits for 0.05 increments to remain true.
            split = 5;
        }
        float totalPiece = total * 1000/split;//increases 1000 times and then reduces by split so that values can be manipulated as integers representations.
        total = total*1000; 
        int pieces = 0;//Variable to keep track of how many pieces/splits were already put in chests. 
        int randNum;
        while(resultsPointer<8){
            randNum = Random.Range(1,split);//generates any amount of splits to be stored 
            if(pieces+randNum<split){ 
                results[resultsPointer] = (totalPiece*randNum)/1000;//reversing to values to store $ float in the chest
                resultsPointer++; //moves to next chest
                pieces = pieces + randNum; 
            } else { //only triggers when the random number is larger than the amount of splits left. Intentional functions this way so that high win games aren't heavily skewed to 7-8 money chests.
                results[resultsPointer] = (totalPiece*(split-pieces))/1000; 
                break;
            }
        }
    }


/* Function to stop the game. In the event of a win (win > 0) winning.cs tween will be called before resetGame() is called from winning.cs. 
 * If game was a bust, resetGame() is called internally and the "Current Balance" winnings.cs stays dormant.
 *
 */
    void stopPlaying(){
            if(win > 0){
                totalWins.GetComponent<Text>().enabled = true;
                totalWins.GetComponent<winnings>().displayWin(win);
            } else{
                resetGame();
            }
    }

/* Function to start the game. Checks if game is already started. Updates values, disables button inputs, generates the results, and starts the chests animating. 
 *
 */
    void startPlaying(){
        if(!playing){
            playing = true;
            disableAll();
            updateBalance(-bet);//updates the balance to remove the amount bet
            updateLastWin();
            generateResults();
            foreach (GameObject chest in chests){
                chest.GetComponent<ChestScript>().gameStart();
            }
        } 
    }

/* Public function to reset the game. Made public so that it can be called after the tween updating Current Balance is finished. Toggles playing, updates values, updates last win and THEN resets the win variable for next game.
 * sets chest pointer back to 1 so that they are ready to be filled, resets clicked chests
 *
 */
    public void resetGame(){
            playing = false;
            updateLastWin();
            updateBalance(win);
            win = 0;
            resultsPointer = 1;
            foreach (GameObject chest in chests){
                chest.GetComponent<ChestScript>().reset();
            }
            //bet = 0.25f; //Uncomment these 2 lines for Current Bet to reset after each game. 
            //betType = 0; //In testing this was not "fun"
            currBetText.text = "Current Bet: $"+ bet.ToString("F2");
            Invoke("enableAll",.6f); //Delayed call to reset the buttons to let tweens and animations run.
        }
}
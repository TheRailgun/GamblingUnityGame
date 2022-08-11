using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script to handle the chest's and its childrens' animations.
 *
 *
 *
 */

public class ChestScript : MonoBehaviour
{
    
    private Vector3 spawnPos; 
    private Animator anim;
    bool clicked = false;
    public GameObject gameMaster; 
    public Sprite redTicket;
    public Sprite greenTicket;
    public SpriteRenderer spriteRenderer;
    
    /* During start we get the animator for the chest this script is attached to. 
     * We freeze the animation and record the spawnPos vector of the Ticket + Text for later use.
     * 
     *
     */
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = 0;    
        spawnPos = spriteRenderer.transform.parent.gameObject.transform.localPosition;
    }


    /* Resets the chest to its orginal state. If the chest was clicked it runs a closing animation. Regardless it calls restart after .5s which finalizes the reset.
     * Also changes the animation controller boolean to that it knows to put the chest back into the waiting animation
     * 
     *
     */

    public void reset(){
        Debug.Log("Chest reseting");
        if(clicked){
            anim.speed = 1;    
            anim.Play("ChestClosing");
            anim.SetBool("isClicked",false); //sets the isClicked boolean in the animation controller so that the animation controller knows to swap states
            clicked = false;
            LeanTween.moveLocal(spriteRenderer.transform.parent.gameObject, spawnPos,.4f);
        }
        Invoke("restart",.5f);
    }

    /* Finalizes returning the chest to its original state by pausing the animation and making the ticket green(the default until a bust occurs)
     *
     */

    void restart(){
        anim.speed = 0;
        spriteRenderer.sprite = greenTicket;
    }

    /* Function to be called from the gameMaster script to start the chests' animating when the "play" begins.
     *
     */

    public void gameStart(){
        anim.speed = 1;
    }

    
    /* In update, if the game is started and this chest is not "clicked" or already open:
     * We check for left click and then check if the left click's coordinates collide with our PolygonCollider
     * If these do collide, we set the value of the ticket to the string given by the getValue function from the gameMaster.
     * If said value == whatever our agreed upon "Pooper" value is, the sprite for the "ticket" is changed.
     * We then change animation in the animator, mark this chest as opened, and tween out the "ticket".
     *
     */

    void Update(){
        if((clicked==false)&&(gameMaster.GetComponent<Game>().getPlaying())){
            
            if(Input.GetMouseButtonDown(0)){
                
                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                PolygonCollider2D coll = this.gameObject.GetComponent<PolygonCollider2D>();

                if(coll.OverlapPoint(wp)){ //if the click occured inside the polygon collider's hitbox.
                    string value = gameMaster.GetComponent<Game>().getValue();
                    this.gameObject.GetComponentInChildren<TextMesh>().text = value;
                    if(value=="Bust!"){
                        spriteRenderer.sprite = redTicket;
                    }
                    clicked = true;
                    anim.SetBool("isClicked",true); //changes the animator boolean which will change the animation state.
                    GameObject text = this.transform.Find("Text").gameObject;
                    LeanTween.moveLocal(text, new Vector3(-0.1f,0,1),.4f).setEase(LeanTweenType.easeOutBack).setDelay(.5f);//delay of .5s so players dont feel rushed or overwhelmed.
                }
            }
        }     
    }
}

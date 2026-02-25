using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    [Header ("Sprite Images")]
    public GameObject spriteRenderer;
    public Sprite[] movementSprite; 
    public SlapstickInput _playerActions;
    Vector2 move, move2;
    InputAction moveAction, secondMoveAction;
    [Header ("Move Variables")]
    [SerializeField] float speed;
    [Space]
    [SerializeField] bool isMoving;

    void Awake(){
        _playerActions = new SlapstickInput();
    }    
    
    private void OnEnable(){
        _playerActions.Enable();

    }

    private void OnDisable(){
        _playerActions.Disable();

    }

    void Start()
    {
        moveAction = _playerActions.Player.Move;
        secondMoveAction = _playerActions.Player.Move2;
        
    }

    private void FixedUpdate(){
        move = moveAction.ReadValue<Vector2>();
        move2 = secondMoveAction.ReadValue<Vector2>();

        var inputY1 = move.y;
        var inputY2 = move2.y;
        var buttonLeft = _playerActions.Player.Left.triggered;
        var buttonRight = _playerActions.Player.Right.triggered;
        var inputRight = move.x > 0 || move2.x > 0;
        var inputLeft = move.x < 0 || move2.x < 0;
        var isIdle = inputY1 == 0 && inputY2 == 0 && (inputRight == false || inputLeft == false);


        if (inputY1 > 0) {
            updateSprite(1);
        } else updateSprite(0);
        if (inputY2 > 0 && inputY1 > 0) {            
            updateSprite(2);
            if (inputY2 == 0 && inputY1 == 0) updateSprite(0);
        }

        //if both up buttons are pressed and a left/right button 
        //then move

        //thanks to: Firnox Create 2D Grid Movement tutorial
        // https://www.youtube.com/watch?v=yYqJZqYMEa4&t=2s
        if (!isMoving){
            if(inputY2 > 0 && inputY1 > 0 && buttonRight){
                speed = 5f;
                transform.localScale = new Vector3 (1,1,1);//flip sprite image by scaling on the x axis
                StartCoroutine(moveEnabled(true));
                isMoving = true;
            }

            if (inputY2 > 0 & inputY1 > 0 && buttonLeft){
                speed = -5f; // reverse direction of movement 
                transform.localScale = new Vector3 (-1,1,1);//flip sprite image by scaling on the x axis
                StartCoroutine(moveEnabled(true)); // move sprite 
                isMoving = true;
            }
        }
        if (isIdle) isMoving = false;

        Teleport();
    }
    //thank you MadMP (my lovie) for teaching me over and over again how to use IEnumerators 
    IEnumerator moveEnabled(bool value){
        if (value) transform.position = new Vector3(
                                            transform.position.x + speed, 
                                            transform.position.y, 
                                            transform.position.z);
        
        yield return null; 
    } 

    IEnumerator moveDown(){
        transform.position = new Vector3 (
                                            transform.position.x, 
                                            transform.position.y - speed, 
                                            transform.position.z);
        yield return null;
    }
    IEnumerator moveUp(){
        transform.position = new Vector3 (
                                            transform.position.x, 
                                            transform.position.y + speed, 
                                            transform.position.z);
        yield return null;
    }
    
    void updateSprite(int pic){
        spriteRenderer.GetComponent<SpriteRenderer>().sprite = movementSprite[pic];
    }

    void Teleport(){
        if (transform.position.x > 50f){
            transform.position = new Vector3 (-40f, transform.position.y,transform.position.z);
            StartCoroutine(moveDown());
        }else if (transform.position.x < -50f) {
            transform.position = new Vector3(40f, transform.position.y, transform.position.z);
            StartCoroutine(moveUp());
        }

        if (transform.position.y < 0) new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
        else if (transform.position.y > 50f) new Vector3(transform.position.x, transform.position.y - 5f, transform.position.z);
    }
}

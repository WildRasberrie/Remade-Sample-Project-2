using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using System.Collections;


public class CarControl : MonoBehaviour
{
    [SerializeField] Transform targetPOS;
    [SerializeField] Transform exitPOS;

    [Space]
    [SerializeField] WheelController[] wheels;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject car;
    [Space]

    [Header("Car Properties")]
    public AnimationCurve torqueCurve;
    public float motorTorque = 3000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f; 
    [Space]

    [Header("Input Actions")]
    public CarInputActions moveAction;
    public PlayerInputActions _playerActions;

    [Space]
    [Header ("Player")]
    [SerializeField] GameObject player;
    [SerializeField] PlayerCamera playerCam;
    //[SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] GameObject camera;
    bool interactTriggered, playerActive; 
    float timePassed; 
    float duration = 1.0f;


    void Awake (){
        moveAction = new CarInputActions();
        _playerActions = new PlayerInputActions();
    }

    private void OnEnable(){
        moveAction.Enable();
        _playerActions.Enable();
    }

    private void OnDisable(){
        moveAction.Disable();
        _playerActions.Disable();

    }

    public void Start(){
        if (playerCam != null)
            playerCam.Initialize(targetPOS);
        else return;
        
    }
    private void FixedUpdate(){

        var interact = moveAction.Car.Interact.triggered;

            //if E key is pressed 
        if (interact && playerActive){
            interactTriggered = true;
        }
        if(interact && !playerActive){
                interactTriggered = false;
                CarMovementDisabled();
            }
        if (interactTriggered){
            
            Vector2 inputVector = moveAction.Car.Movement.ReadValue<Vector2>();
            var input = _playerActions.Gameplay;

            CarMovementEnabled();
            //player input for accel and steering 
            var vInput = inputVector.y;
            var hInput = inputVector.x;

            //calc current speed along with car forward axis 
            var forwardSpeed = Vector3.Dot(transform.forward, rb.linearVelocity);
            var speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed));

                //apply front wheels torque (whatever that means) and steering at 
                //high speeds for better handling  
                float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
                float currentSteeringRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

                //determine if the player is accelerating for trying to reverse
                bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

                timePassed += Time.deltaTime;
                var timeNormalized = Mathf.Clamp01(timePassed/duration);
                var curveTracker = torqueCurve.Evaluate(timeNormalized);
                
                foreach (var wheel in wheels){
                    //apply steering to wheels that support steering
                    if (wheel.steerable){
                        wheel.WheelCollider.steerAngle = hInput * currentSteeringRange;
                    }

                    if(isAccelerating){
                        //Apply torque to motorized wheels
                        if(wheel.motorized){
                            wheel.WheelCollider.motorTorque = vInput * currentMotorTorque * curveTracker;
                        }
                        //release brake when accel 
                        wheel.WheelCollider.brakeTorque = 0f; 
                    }else{
                        //Apply brakes when reversing direction
                        wheel.WheelCollider.motorTorque = 0f;
                        wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                    }
                }
            }
        }
    void CarMovementEnabled(){
        Vector2 inputVector = moveAction.Car.Movement.ReadValue<Vector2>();
        var input = _playerActions.Gameplay;
        //move to main camera to be child of car
        camera.transform.parent = car.transform;

           // turn off player 
        player.SetActive(false);
        playerActive = false;
            //make camera follow behind car at offset 
            //turn on playerCam script
        playerCam.enabled = true; 
        var cameraInput = new CameraInput{
                Look = input.Look.ReadValue<Vector2>(),
                Move = inputVector
            };

        playerCam.UpdateRotation(cameraInput);
        playerCam.UpdatePosition(targetPOS);
    }

    void CarMovementDisabled(){
        player.SetActive(true);
        playerActive = true;
        player.transform.localPosition = exitPOS.localPosition;
        playerCam.enabled = false;
        camera.transform.parent = player.transform;

    }
}

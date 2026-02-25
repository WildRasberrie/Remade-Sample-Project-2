using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] PlayerCamera playerCamera;
    PlayerInputActions _inputActions;
    Animator animator;
    Vector3 originRot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originRot = playerCamera._eulerAngles;
        animator = GetComponent<Animator>();
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        
        // lock cursor to center
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDestroy(){
        _inputActions.Dispose();

    }
    // Update is called once per frame
    void Update(){
        var input = _inputActions.Gameplay; 

        //get cam input and update its rot 
        var cameraInput = new CameraInput{Look = input.Look.ReadValue<Vector2>()};
        playerCamera.UpdateRotation(cameraInput);

        //grab character input & update it 
        var characterInput = new CharacterInput{
            Rotation = playerCamera.transform.rotation, 
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Crouch = input.Crouch.WasPressedThisFrame()
                ? CrouchInput.None
                : CrouchInput.None
            };

        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(Time.deltaTime);
    }

    void LateUpdate(){
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());

        var animationName = animator.GetCurrentAnimatorStateInfo(0);

        if(playerCharacter._requestedMovement != Vector3.zero){
            animator.Play("Walk");
            
            if (playerCamera._eulerAngles != originRot) animator.Play("Walk");
        } else animator.Play("Idle");
    }
}

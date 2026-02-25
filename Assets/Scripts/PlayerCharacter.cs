using UnityEngine;
using KinematicCharacterController;

public enum CrouchInput{
    None, Toggle
}

public enum Stance{
    Stand, Crouch
}


public struct CharacterInput{
    public Quaternion Rotation;

    public Vector2 Move;

    public bool Jump;

    public bool JumpSustain;

    public CrouchInput Crouch;

}
public class PlayerCharacter : MonoBehaviour, ICharacterController
{

    [SerializeField] KinematicCharacterMotor motor;

    [SerializeField] Transform root;

    [SerializeField] Transform cameraTarget;

    [Space]

    [SerializeField] float walkSpeed = 20f;

    [Space]

    [SerializeField] float crouchSpeed = 7f;

    [SerializeField] float walkResponse = 25f;
    [SerializeField] float crouchResponse = 20f;

    [Space]
    
    [SerializeField] float airSpeed = 15f;
    [SerializeField] float airAccel = 70f;

    [Space]

    [SerializeField] float jumpSpeed = 20f;

    [Space]

    [Range (0f, 1f)]
    [SerializeField] float jumpSustainGravity = 0.4f;
    [SerializeField] float gravity = -90f; 

    [Space]

    [SerializeField] float standHeight = 2f;
    [SerializeField] float crouchHeight = 1f;

    [SerializeField] float crouchHeightResponse = 15f;

    [Space]
    [Range(0,1f)]
    [SerializeField] float standCameraTargetHeight = 0.9f;
    [Range(0,1f)]
    [SerializeField] float crouchCameraTargetHeight = 0.7f;

    private Stance _stance;

    Quaternion _requestedRotation;
    
    public Vector3 _requestedMovement;

    bool _requestedJump;

    bool _requestedSustainedJump;

    bool _requestedCrouch;

    Vector3  camOffset = new Vector3(0f, 0.5f, -3f);

    Collider[] _uncrouchOverlapResults;

    public void Initialize(){
        _stance = Stance.Stand;
        _uncrouchOverlapResults = new Collider[8];

        motor.CharacterController = this;
    }

     public void UpdateInput(CharacterInput input){
        _requestedRotation = input.Rotation;
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
        // Orient pos to direction the cam is facing
        _requestedMovement = input.Rotation * _requestedMovement;

        _requestedJump = _requestedJump || input.Jump;
        _requestedSustainedJump = input.JumpSustain;

        _requestedCrouch = input.Crouch switch {
            CrouchInput.Toggle => !_requestedCrouch,
            CrouchInput.None => _requestedCrouch,
            _ => _requestedCrouch

        };
    }

    public void UpdateBody(float deltaTime){
        var currentHeight = motor.Capsule.height;
        var normalizedHeight = currentHeight / standHeight;

        var cameraTargetHeight = currentHeight * (
            _stance is Stance.Stand
            ? standCameraTargetHeight
            : crouchCameraTargetHeight
        );

        var rootTargetScale = new Vector3(1f, normalizedHeight, 1f);

        cameraTarget.localPosition = Vector3.Lerp(
            a: cameraTarget.localPosition,
            //add third person perspective coords here
            b: new Vector3(0f, cameraTargetHeight + camOffset.y, camOffset.z),
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
        );

        root.localScale = Vector3.Lerp(
            a: root.localScale,
            b: rootTargetScale,
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
        );
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime){

        var forward = Vector3.ProjectOnPlane (
            _requestedRotation * Vector3.forward, 
            motor.CharacterUp
            );
        if (forward != Vector3.zero){
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
         }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime){
        if (motor.GroundingStatus.IsStableOnGround){
            var groundedMovement = motor.GetDirectionTangentToSurface(
                direction: _requestedMovement, 
                surfaceNormal : motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            var speed = _stance is Stance.Stand 
                    ? walkSpeed
                    : crouchSpeed;

            var response = _stance is Stance.Stand 
                    ? walkResponse
                    : crouchResponse;
            
            var targetVelocity = groundedMovement * speed ;

            currentVelocity = Vector3.Lerp(
                a: currentVelocity,
                b: targetVelocity,
                t: 1f- Mathf.Exp(-response *deltaTime)
            );

        }else{
            //Move
            if (_requestedMovement.sqrMagnitude > 0f){
                var planarMovement = Vector3.ProjectOnPlane(
                vector: _requestedMovement,
                planeNormal: motor.CharacterUp
                ) * _requestedMovement.magnitude;

                //current vel on movement plane
                var currentPlanarVelocity = Vector3.ProjectOnPlane(
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );

                //calc movement force 
                var movementForce = planarMovement * airAccel * deltaTime;
            
                //add target vel
                var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                currentVelocity += targetPlanarVelocity - currentPlanarVelocity;

            }
            //Gravity
            var effectiveGravity = gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity,motor.CharacterUp);

            if (_requestedSustainedJump && verticalSpeed > 0f){
                effectiveGravity = jumpSustainGravity;
            }
            currentVelocity += motor.CharacterUp * effectiveGravity * deltaTime;
        }

        


        if (_requestedJump){
            _requestedJump = false; 
            motor.ForceUnground(time: 0f);

            currentVelocity += motor.CharacterUp * jumpSpeed;
            // set min vertical speed to jump speed
            var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);

            currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            
        }
        
    }

    public void BeforeCharacterUpdate(float deltaTime){
        //crouch
        if (_requestedCrouch && _stance is Stance.Stand){
            _stance = Stance.Crouch;

            motor.SetCapsuleDimensions(
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
            );
        }
    }
    public void PostGroundingUpdate(float deltaTime){

    }

    public void AfterCharacterUpdate(float deltaTime){
        //uncrouch
        if (!_requestedCrouch && _stance is not Stance.Stand){

            motor.SetCapsuleDimensions(
                radius: motor.Capsule.radius,
                height: standHeight,
                yOffset: standHeight * 0.5f
            );

            var pos= motor.TransientPosition;
            var rot = motor.TransientRotation;
            var mask = motor.CollidableLayers;
        
            if (motor.CharacterOverlap(pos,rot, _uncrouchOverlapResults, mask, QueryTriggerInteraction.Ignore) > 0){
                _requestedCrouch = true; 

                motor.SetCapsuleDimensions(
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
                );
            } else {
                _stance = Stance.Stand;
            }
        }
    }
  

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}
    public bool IsColliderValidForCollisions(Collider coll) => true;
        
    public void OnDiscreteCollisionDetected(Collider hitCollider){}
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport){}

    public Transform GetCameraTarget() => cameraTarget;

   
}

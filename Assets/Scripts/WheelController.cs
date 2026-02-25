using UnityEngine;
using UnityEngine.InputSystem;
// reference from Unity Documentation 
//https://docs.unity3d.com/6000.3/Documentation/Manual/WheelColliderTutorial.html
public class WheelController : MonoBehaviour
{
   public Transform wheelModel;

   [HideInInspector] public WheelCollider WheelCollider;
    //create properties for the carControl script
    // Enable/Disable these through the Editor Inspector Window
   public bool steerable;
   public bool motorized;

   Vector3 position;
   Quaternion rotation;

   private void Start(){
        WheelCollider = GetComponent<WheelCollider>();
   }

   void Update(){
        //Get the wheel collider's pose values and 
        //use them to set wheel model's pos and rot
        Quaternion wheelRot = new Quaternion(0f,0f,0.71f,0.71f);

         WheelCollider.GetWorldPose(out position, out rotation);
                 wheelModel.transform.position = position;
     //            wheelModel.transform.rotation = rotation * wheelRot;
        
   }
}


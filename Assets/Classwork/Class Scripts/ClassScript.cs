using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class ClassScript : MonoBehaviour
{
    public Transform pointObject;
    public Transform parentObj;
    [Header("Parabola Values")]
    [SerializeField] int amount = 20;
    [SerializeField] float a,b,c;
    [Header("Circle Values")]
    [SerializeField]int r;
    [SerializeField]int center;
    bool timeOut;
    int num =0;
    [Space]
    [Header( "Materials")]
    [SerializeField] Material pointPrefab;
    [SerializeField] Color materialColor;
    [SerializeField] Color materialColor2;
    [SerializeField] float timeElasped = 2f;
    List<GameObject> points = new List<GameObject>();
    int index = 0;
    float gravity = 1f; 
    Vector3 initPOS;
    [SerializeField] float duration;
    bool pressed = false;
    void Start(){
        for (int i = 0; i < amount; i ++){
            //Instantiate(pointObject, , Quaternion.identity,parentObj);
            Instantiate(pointObject, DrawCircle(i), Quaternion.identity,parentObj);
        }
        GameObject[] clone = GameObject.FindGameObjectsWithTag("point");

        foreach(GameObject test in clone){
            points.Add(test);
        }
        initPOS = new Vector3(transform.position.x,transform.position.y);
    }
    
    void Update(){
        TimeOut(timeElasped);
        if(timeOut)
        {
           timeElasped = 2f;
           pressed = false;
        }

        var pointTagged = GameObject.FindGameObjectsWithTag("point");

        for (int i = 0; i < points.Count; i ++){
            points[i].transform.rotation = RotateOverTime(10);

        }

        if (Input.GetKey(KeyCode.Space)){ 
            pressed = true; 
             
        }

        print(pressed);
        
        if (pressed){
            for (int i =0; i < pointTagged.Length; i++){
                num = i %2;
                if(i == index)  pointTagged[i].transform.position = new Vector3(initPOS.x, initPOS.y + EaseOffScreen());
            }
            var currentYPOS = points[num].transform.position.y;
            points[num].transform.position = new Vector3 (points[num].transform.position.x,points[num].transform.position.y - gravity);  
        
        //  make point hit the floor when pressing space
            if (currentYPOS <-20f){
                var step = 0f;
                step += 1f;
                points[num].transform.position = new Vector3(points[num].transform.position.x + step,
                                                                Bounce(currentYPOS));
            } 
            
            //decrease the duration by time, when duration hits 0 stop
            duration -= Time.deltaTime;
            if (duration < 0f) duration = 0f;
        }
    }
    //Equation 1:
    Vector3 DrawCircle(int i){
       //equation for a circle 
       // y = 1 + r * sin(t)
        var y = new Vector3(center + a + (r * Mathf.Cos(i)),center + (r * Mathf.Sin(i)));  
        return y;
    }
    
    void TimeOut(float timePassed){
            timePassed -= Time.deltaTime;
            if (timePassed < 0) timeOut = true; else timeOut = false ;

        }

    void ChangeColor(GameObject gameObj,Color mat){
        var colorChanger = gameObj.GetComponent<Renderer>().material;
        colorChanger.color = mat;
    }
    float createParabola(float x){
        return a* Mathf.Pow(x+b,2) + c;
    }

    //Equation 2: 
// create bounce effect
    float Bounce(float initPOSy){
        //bounce equation :  y(t)= amplitude * |sin (t + speed)|
        // used to calculate the height of the next bounce
        //amplitude = intial height 
        var speed = 0.02f;         

        float y = initPOSy * Mathf.Abs(Mathf.Sin(speed + duration));
        return y;
    }

    //Equation 3:
    //Make parabola ease off screen 
    float EaseOffScreen(){
        //clamp time from 0 to 1
        var time = Mathf.InverseLerp(0,1, duration);
        return 1f - Mathf.Sin(time * (Mathf.PI/2f));
    }

    //Equation 4: 
    Quaternion RotateOverTime(int dur){
        var startRot = transform.rotation;
       
        startRot = new Quaternion(startRot.x + (360f *Time.deltaTime/dur), startRot.y, startRot.z, startRot.w);
        
       return startRot;
    }

}
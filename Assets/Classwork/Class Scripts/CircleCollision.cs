using UnityEngine;
using UnityEngine.UI;

public class CircleCollision : MonoBehaviour
{
    /**

    FOR UNITY6 >>>> BEFORE USING THIS CODE MAKE SURE THE INPUT SYSTEM UNDER PLAYER SETTINGS IS SET TO "BOTH" <<<
    
    **/

    //circle prefab will be the base shape/sprite for both circles in our screen
    [SerializeField]
    GameObject circlePrefab;

    //our two circles, each will have their own radius
    GameObject[] circle1;
    GameObject circle2;

    [Header ("Target Positions")]
    [SerializeField] Transform[] targetPOS;
    [Header ("Enemy Health Values")]
    [SerializeField] Slider[] enemyHealth;
    [SerializeField] RectTransform[] healthPOS;
    [SerializeField] GameObject[] healthColor;

    [Header ("Parent")]
    [SerializeField] Transform parent;
    public float radius1;
    public float radius2;

    void Start()
    {
        //instantiate both circles in the scene
        circle1 = new GameObject[targetPOS.Length];
        circle2 = Instantiate(circlePrefab);

        for (int i = 0; i < targetPOS.Length; i++){
            circle1[i] = Instantiate(circlePrefab, targetPOS[i].position, Quaternion.identity, parent);
            circle1[i].transform.localScale = Vector3.one * radius1 * 2;
            //set enemy health position to be above enemy positions 
            healthPOS[i].position = new Vector2 (targetPOS[i].position.x + 1.5f, targetPOS[i].position.y + 5f);

        }

        //scale the circles according to their radius
        circle2.transform.localScale = Vector3.one * radius2 * 2;
    }

    // Update is called once per frame
    void Update()
    {
        //one of the circles will follow your mouse
        Vector3 pos = Input.mousePosition;
        pos.z = 30f;
        circle2.transform.position = Camera.main.ScreenToWorldPoint(pos);

        //run the collision detection function
        CheckCollision();
        // Change the Health Value 
        healthValueChanger();
    }

    void CheckCollision(){
        bool changeHealth;
        for (int i =0; i < targetPOS.Length; i ++){
            var c2 = circle2.transform.position;
            //do this part in class
            if (distance(new Vector2 (circle1[0].transform.position.x,circle1[0].transform.position.y),c2,radius1, radius2)){
                print("Collision Detected!");
                circle1[0].GetComponent<SpriteRenderer>().color = Color.red;
                //change effected circle health value 
                enemyHealth[0].value -= 1;
            }
            else{
                circle1[0].GetComponent<SpriteRenderer>().color = Color.white;
            }
            if (distance(new Vector2 (circle1[1].transform.position.x,circle1[1].transform.position.y),c2,radius1, radius2)){
                print("Collision Detected!");
                circle1[1].GetComponent<SpriteRenderer>().color = Color.red;
                //change effected circle health value 
                enemyHealth[1].value -= 1;
            } else{
                circle1[1].GetComponent<SpriteRenderer>().color = Color.white;
            }
             if (distance(new Vector2(circle1[2].transform.position.x,circle1[2].transform.position.y),c2,radius1, radius2)){
                print("Collision Detected!");
                circle1[2].GetComponent<SpriteRenderer>().color = Color.red;
                //change effected circle health value 
                enemyHealth[2].value -= 1;
            } else{
                circle1[2].GetComponent<SpriteRenderer>().color = Color.white;
            }

           
        }
        
    }

    void healthValueChanger(){
        for (int i = 0; i < healthPOS.Length; i ++){
            if (enemyHealth[i].value <50) healthColor[i].GetComponent<Image>().color = Color.red;
        }
        if (enemyHealth[0].value < 1){
            circle1[0].transform.position += Vector3.up; 
            healthPOS[0].position =  circle1[0].transform.position;
        }
        if (enemyHealth[1].value < 1){ 
            circle1[1].transform.position += Vector3.up; 
            healthPOS[1].position =  circle1[1].transform.position;
        }
        if (enemyHealth[2].value < 1) {
            circle1[2].transform.position += Vector3.up; 
            healthPOS[2].position =  circle1[2].transform.position;
        }
    }

    bool distance(Vector2 circle1, Vector2 circle2, float radius1, float radius2) {
        var deltaPOS = circle2 - circle1;
        float dist = deltaPOS.magnitude;
        return dist < (radius1 + radius2);
    }
}

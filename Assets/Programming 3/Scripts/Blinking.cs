using UnityEngine;
using System.Collections;

public class Blinking : MonoBehaviour
{
    SpriteRenderer renderer; 
    [SerializeField] Color endColor; 
    [SerializeField] Color startColor; 
    [SerializeField] float duration = 1f;
    [SerializeField] bool looped;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        //if (renderer != null) startColor = renderer.material.color;
    }

    void Update(){
        StartCoroutine(Blink());
    }

    private IEnumerator Blink(){
        float timePassed = 0f;
        bool part1 = false;
        if (!part1){
            while (timePassed < duration/2){
                renderer.material.color = Color.Lerp(startColor,endColor,(timePassed/(duration/2)));
                timePassed += Time.deltaTime; 
                part1 = true;
            }
        }
        
        if (part1){
            timePassed = 0f; 

            while (timePassed < duration/2){
                renderer.material.color = Color.Lerp(endColor,startColor,(timePassed/duration/2));
                timePassed += Time.deltaTime; 
                yield return null;
            }
            part1 = false; 
        }
    }
}

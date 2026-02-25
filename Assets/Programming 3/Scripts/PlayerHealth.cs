using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Slider playerHealth;
    [SerializeField] float value;
    [Header ("Colliders")]
    public Collider2D player; 
    public Collider2D enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        value = playerHealth.value;
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (value == 0) Destroy()

    }

    void OnCollisionEnter(Collision col){
        if (col.gameObject.CompareTag("Enemy")){
            value -= 2f;
        }

    }
}

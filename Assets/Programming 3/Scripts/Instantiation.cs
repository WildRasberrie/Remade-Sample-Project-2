using UnityEngine;
using System.Collections.Generic;
public class Instantiation : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject parentObj;
    [SerializeField] Transform target;
    [SerializeField] float amount;
    [SerializeField] float timePassed = 0f;
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > 10f){
            for (int i =0; i < amount; i++){
                var clone = Instantiate(prefab,target.position, Quaternion.identity, parentObj.transform);
                Destroy(clone, 10f);
            }
            timePassed = 0f;
        }
    }
}

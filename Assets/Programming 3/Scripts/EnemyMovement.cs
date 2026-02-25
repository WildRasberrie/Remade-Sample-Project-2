using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform player;
    [SerializeField] Transform parent;
    [SerializeField] float speed = 4f;
    [SerializeField] bool reachedTarget;
    void Update()
    {
        if (parent.childCount > 0){
            target = parent.GetChild(0);

            Vector2 direction = (target.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        } else return;

        // Distance(transform.position, player.position);
        // if (reachedTarget) target.position = new Vector3 (0,500f,0);
    }

     void Distance(Vector3 pos, Vector3 targetPos){
        var offset = 5f;
        if ((pos.x <= targetPos.x + offset && pos.x >= targetPos.x) || 
        (pos.x >= targetPos.x - offset && pos.x <= targetPos.x) ||
        (pos.y <= targetPos.y + offset && pos.x >= targetPos.y) || 
        (pos.y >= targetPos.y - offset && pos.y <= targetPos.y)){
            reachedTarget = true; 
        } else reachedTarget = false;

    }
}

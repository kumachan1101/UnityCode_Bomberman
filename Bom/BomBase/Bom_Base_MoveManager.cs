using UnityEngine;

public class Bom_Base_MoveManager : MonoBehaviour
{
    private Vector3 moveDirection = Vector3.zero;
    private float moveSpeed = 1.5f; // 動く速さ
    private bool isMoving = false;
    Bom_Base_CollisionManager cCollisionManager;

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void SetCollisionManager(Bom_Base_CollisionManager cManager){
        cCollisionManager = cManager;
    }

    public void Move(Transform transform)
    {
        if(GameManager.xmax <= transform.position.x || GameManager.zmax <= transform.position.z || 0 > transform.position.x || 0 > transform.position.z){
            return;
        }

        if (isMoving)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime * 2;
            if(cCollisionManager.CheckForCollision(transform.position, moveDirection)){
                // 衝突を検知したら座標を補正して移動を止める
                transform.position = Library_Base.GetPos(transform.position);
                StopMoving(); // 移動停止
            }
        }
    }

    public void AbailableBomKick()
    {
        StartMoving();
    }

    public void AbailableBomAttack(Vector3 direction)
    {
        StartMoving();
        SetMoveDirection(direction);
    }
}

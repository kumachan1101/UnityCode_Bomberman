using UnityEngine;

public class Bom_Base_MoveManager : MonoBehaviour
{
    private Vector3 moveDirection = Vector3.zero;
    private float moveSpeed = 1.5f; // 動く速さ
    private bool isMoving = false;

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

    public void Move(Transform transform)
    {
        if(GameManager.xmax <= transform.position.x || GameManager.zmax <= transform.position.z || 0 > transform.position.x || 0 > transform.position.z){
            return;
        }

        if (isMoving)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime * 2;
            CheckForCollision(transform);
        }
    }

    public void CheckForCollision(Transform transform)
    {
        // 移動方向にレイを飛ばして衝突を検知
        RaycastHit hit;
        if (Physics.Raycast(transform.position, moveDirection, out hit, 1f))
        {
            // 衝突したオブジェクトの名前によって処理を分岐する
            switch (hit.transform.name)
            {
                case "Broken(Clone)":
                case "FixedWall(Clone)":
                case "Wall(Clone)":
                case "Bom(Clone)":
                case "Bombigban(Clone)":
                case "BomExplode(Clone)":
                    // 衝突を検知したら座標を補正して移動を止める
                    transform.position = Library_Base.GetPos(transform.position);
                    StopMoving(); // 移動停止
                    break;
                default:
                    // 上記の条件に該当しない場合は何もしない
                    return;
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

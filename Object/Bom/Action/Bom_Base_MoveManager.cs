using UnityEngine;
public interface IBomMoveState
{
    void Execute(Transform transform);
}

public class BomStoppedState : IBomMoveState
{
    public void Execute(Transform transform)
    {
        transform.position = Library_Base.GetPos(transform.position);
    }
}

public class BomMovingState : IBomMoveState
{
    private float moveSpeed = 1.5f;

    public BomMovingState(int iSpeed){
        moveSpeed = iSpeed;
    }
    public void Execute(Transform transform)
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime * 2;
    }
}

public class BomMover : MonoBehaviour
{
    IBomMoveState currentState = new BomStoppedState();
    public void ReqMove(Vector3 direction, int iSpeed)
    {
        if (direction == Vector3.zero) return; // 無効
        transform.forward = direction;
        currentState = new BomMovingState(iSpeed);
    }

    void Update()
    {
        currentState.Execute(transform);
    }

    public void ForceStop()
    {
        currentState = new BomStoppedState();
    }
}

public class Bom_Base_MoveManager : MonoBehaviour
{
    private BomMover mover;
    private BomStatusData status;
    private Bom_Base_CollisionManager cCollisionManager;

    private void Awake()
    {
        cCollisionManager = gameObject.AddComponent<Bom_Base_CollisionManager>();
        mover = gameObject.AddComponent<BomMover>();
    }

    public void BomAttack(BomParameters bomParams)
    {
        UpdateBomStatus(bomParams);
        if(CanBomAttack()){
            mover.ReqMove(bomParams.direction, bomParams.iSpeed);
        }
    }

    private void UpdateBomStatus(BomParameters bomParams){
        // ボム複数ドロップアイテム取得済みの場合は、ボムアタックを実行しない
        status = new BomStatusData(bomParams);
    }

    private bool CanBomAttack(){
        // ボム複数ドロップアイテム取得済みの場合は、ボムアタックを実行しない
        if(BOM_ATTACK.BOM_ATTACK_THROW == status.bomAttack){
            return true;
        }

        return false;
    }

    public void BomKick(Vector3 direction, int iSpeed)
    {
        if(status.bomKick){
            mover.ReqMove(direction, iSpeed);
        }
    }

    void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (IsOutOfBounds() || HasCollision())
        {
            mover.ForceStop();
        }
    }

    private bool IsOutOfBounds()
    {
        Vector3 pos = transform.position;
        return pos.x < 0 || pos.z < 0 || pos.x >= GameManager.xmax || pos.z >= GameManager.zmax;
    }

    private bool HasCollision()
    {
        return cCollisionManager.CheckForCollision();
    }
    public void Explosion()
    {
        mover.ForceStop();
    }
}

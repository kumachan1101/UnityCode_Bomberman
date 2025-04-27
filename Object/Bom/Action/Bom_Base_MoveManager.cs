/*
using Unity.VisualScripting;
using UnityEngine;

public class Bom_Base_MoveManager : MonoBehaviour
{
    private Vector3 moveDirection = Vector3.zero;
    private float moveSpeed = 1.5f; // 動く速さ
    private bool isMoving = false;
    private bool bBomKick = false;
    private bool bBomAttack = false;

    Bom_Base_CollisionManager cCollisionManager;

    public void ReqBomAttack(Vector3 direction)
    {
        //BomAttack実行可能クラス生成、コンストラクタ方向
        if(bBomAttack){
            StartMoving();
            moveDirection = direction;
        }
    }

    public void ReqBomKick(Vector3 direction)
    {
        //BomKick実行可能クラス生成、コンストラクタ方向
        if(bBomKick){
            StartMoving();
            moveDirection = direction;
        }
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
        //BomKickチェック関数実行
        //チェック結果停止条件になったら、停止クラス生成
        //チェック結果停止条件にならなかったら、実行
        
        //BomAttackチェック関数実行
        //チェック結果停止条件になったら、停止クラス生成
        //チェック結果停止条件にならなかったら、実行
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

    public void ReqSetting(BomParameters bomParams){
        AbailableBomKick(bomParams);
        AbailableBomAttack(bomParams);
    }

    public void AbailableBomKick(BomParameters bomParams)
    {
        //BomKickクラスが生成されていなかったら
        if(false == bomParams.bomKick){
            return;
        }
        //StartMoving();
        //BomKick未実行クラス生成、コンストラクタなし
        bBomKick = true;
    }

    public void AbailableBomAttack(BomParameters bomParams)
    {
        //BomAttackクラスが生成されていなかったら
        if(false == bomParams.bomAttack){
            return;
        }
        //StartMoving();
        //BomAttack未実行クラス生成、コンストラクタなし
        bBomAttack = true;
        ReqBomAttack(bomParams.direction);
    }
}
*/
/*
using UnityEngine;

public class Bom_Base_MoveManager : MonoBehaviour
{
    private Vector3 moveDirection = Vector3.zero;
    private float moveSpeed = 1.5f;

    private IKickState kickState = new KickUnavailableState();
    private IAttackState attackState = new AttackUnavailableState();

    private Bom_Base_CollisionManager cCollisionManager;

    public void DropBom(BomParameters bomParams)
    {
        BomStatusData status = new BomStatusData(bomParams);
        status.isDropped = true;

    }

    public void BomKick(Vector3 direction)
    {
        BomStatusData status = new BomStatusData();
        status.direction = direction;
        status.bomKick = true;
    }

    public void ReqBomAttack(Vector3 direction)
    {
        ChangeAttackState(new BomAttackState(direction));
    }

    public void SetCollisionManager(Bom_Base_CollisionManager cManager)
    {
        cCollisionManager = cManager;
    }

    public void Move(Transform transform)
    {
        if (GameManager.xmax <= transform.position.x || GameManager.zmax <= transform.position.z ||
            0 > transform.position.x || 0 > transform.position.z)
        {
            return;
        }

        kickState.Move(transform);
        attackState.Move(transform);
    }

    public void StopMoving()
    {
        ChangeAttackState(new AttackUnavailableState());
        ChangeKickState(new KickUnavailableState());
    }

    public void ChangeKickState(IKickState newState)
    {
        kickState.Exit();
        kickState = newState;
        kickState.Enter(this);
    }

    public void ChangeAttackState(IAttackState newState)
    {
        attackState.Exit();
        attackState = newState;
        attackState.Enter(this);
    }

    public bool CheckCollision(Vector3 pos, Vector3 dir)
    {
        return cCollisionManager != null && cCollisionManager.CheckForCollision(pos, dir);
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    public void ApplyMovement(Transform transform)
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime * 2;
    }

    public void CorrectPosition(Transform transform)
    {
        transform.position = Library_Base.GetPos(transform.position);
    }

    public void AbailableBomKick(BomParameters bomParams)
    {
        if (bomParams.bomKick)
        {
            ChangeKickState(new KickIdleState());
        }
    }

    public void AbailableBomAttack(BomParameters bomParams)
    {
        if(bomParams.bomAttack){
            ReqBomAttack(bomParams.direction);
        }
    }

}


// Kick States
public interface IKickState
{
    void Enter(Bom_Base_MoveManager manager);
    void Move(Transform transform);
    void Exit();
    void OnRequest(Vector3 direction);
}

public class KickUnavailableState : IKickState
{
    public void Enter(Bom_Base_MoveManager manager) { }
    public void Move(Transform transform) { }
    public void Exit() { }
    public void OnRequest(Vector3 direction) { }
}

public class KickIdleState : IKickState
{
    private Bom_Base_MoveManager manager;
    public void Enter(Bom_Base_MoveManager manager)
    {
        this.manager = manager;
    }
    public void Move(Transform transform) { }
    public void Exit() { }
    public void OnRequest(Vector3 direction)
    {
        manager.ChangeKickState(new BomKickState(direction));
    }
}

public class BomKickState : MoveOnceStateBase, IKickState
{
    public BomKickState(Vector3 direction) : base(direction) { }

    public override void Enter(Bom_Base_MoveManager manager)
    {
        base.Enter(manager);
    }

    protected override void OnHitCollision()
    {
        manager.ChangeKickState(new KickIdleState());
    }

    public void OnRequest(Vector3 direction) { }
}


// Attack States
public interface IAttackState
{
    void Enter(Bom_Base_MoveManager manager);
    void Move(Transform transform);
    void Exit();
    void OnRequest(Vector3 direction);
}

public class AttackUnavailableState : IAttackState
{
    public void Enter(Bom_Base_MoveManager manager) { }
    public void Move(Transform transform) { }
    public void Exit() { }
    public void OnRequest(Vector3 direction) { }
}

public class AttackIdleState : IAttackState
{
    private Bom_Base_MoveManager manager;
    public void Enter(Bom_Base_MoveManager manager)
    {
        this.manager = manager;
    }
    public void Move(Transform transform) { }
    public void Exit() { }
    public void OnRequest(Vector3 direction)
    {
        manager.ChangeAttackState(new BomAttackState(direction));
    }
}

public class BomAttackState : MoveOnceStateBase, IAttackState
{
    public BomAttackState(Vector3 direction) : base(direction) { }

    public override void Enter(Bom_Base_MoveManager manager)
    {
        base.Enter(manager);
    }

    protected override void OnHitCollision()
    {
        manager.ChangeAttackState(new AttackIdleState());
    }

    public void OnRequest(Vector3 direction) { }
}


// MoveOnceStateBase.cs
public abstract class MoveOnceStateBase
{
    protected Vector3 direction;
    protected Bom_Base_MoveManager manager;

    public MoveOnceStateBase(Vector3 direction)
    {
        this.direction = direction;
    }

    public virtual void Enter(Bom_Base_MoveManager manager)
    {
        this.manager = manager;
        manager.SetMoveDirection(direction);
    }

    public virtual void Move(Transform transform)
    {
        manager.ApplyMovement(transform);
        if (manager.CheckCollision(transform.position, direction))
        {
            manager.CorrectPosition(transform);
            OnHitCollision();
        }
    }

    protected abstract void OnHitCollision();

    public virtual void Exit() { }
}
*/

using UnityEditor.Experimental.GraphView;
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

    public void Execute(Transform transform)
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime * 2;
    }
}

public class BomMover : MonoBehaviour
{
    IBomMoveState currentState = new BomStoppedState();
    public void ReqMove(Vector3 direction)
    {
        if (direction == Vector3.zero) return; // 無効
        transform.forward = direction;
        currentState = new BomMovingState();
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
        if(CanBomAttack(bomParams)){
            mover.ReqMove(bomParams.direction);
        }
    }

    private bool CanBomAttack(BomParameters bomParams){
        // ボム複数ドロップアイテム取得済みの場合は、ボムアタックを実行しない
        status = new BomStatusData(bomParams);
        if(BOM_ATTACK.BOM_ATTACK_THROW == status.bomAttack){
            return true;
        }

        return false;
    }

    public void BomKick(Vector3 direction)
    {
        if(status.bomKick){
            mover.ReqMove(direction);
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

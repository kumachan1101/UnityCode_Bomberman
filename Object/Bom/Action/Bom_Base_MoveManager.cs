using System;
using UnityEngine;

/// <summary>
/// ボムの設置・移動で共通利用するグリッド変換。
/// プレイヤーが斜めを向いていても、必ず縦横どちらか1方向の別マスへ進める。
/// </summary>
public static class BomGridRules
{
    public static Vector3 ToCell(Vector3 position)
    {
        return Library_Base.GetPos(position);
    }

    public static Vector3 GetCardinalDirection(Vector3 direction)
    {
        float absoluteX = Mathf.Abs(direction.x);
        float absoluteZ = Mathf.Abs(direction.z);
        if (absoluteX <= Mathf.Epsilon && absoluteZ <= Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        if (absoluteX > absoluteZ)
        {
            return direction.x >= 0f ? Vector3.right : Vector3.left;
        }

        return direction.z >= 0f ? Vector3.forward : Vector3.back;
    }

    public static Vector3 GetCellInDirection(Vector3 origin, Vector3 direction, int distance)
    {
        Vector3 cell = ToCell(origin);
        Vector3 cardinalDirection = GetCardinalDirection(direction);
        return cell + cardinalDirection * Mathf.Max(0, distance);
    }

    public static bool IsBombAtCell(Vector3 position, GameObject ignoredObject = null)
    {
        return Bom_Base.IsActiveBomAtCell(position, ignoredObject);
    }
}

/// <summary>
/// 最後に到達した安全なマスを保持し、障害物が現れた場合もそのマスへ戻して停止する。
/// </summary>
public class BomMover : MonoBehaviour
{
    private float moveSpeed = 1.5f;
    private Vector3 moveDirection;
    private Vector3 settledCell;
    private bool moving;

    public bool IsMoving { get { return moving; } }
    public Vector3 SettledCell { get { return settledCell; } }

    public void ReqMove(Vector3 direction, int speed)
    {
        Vector3 cardinalDirection = BomGridRules.GetCardinalDirection(direction);
        if (cardinalDirection == Vector3.zero)
        {
            return;
        }

        moveDirection = cardinalDirection;
        moveSpeed = Mathf.Max(0.1f, speed);
        settledCell = BomGridRules.ToCell(transform.position);
        transform.position = settledCell;
        transform.forward = cardinalDirection;
        moving = true;
    }

    public void Advance(float deltaTime, Func<Vector3, bool> isCellBlocked)
    {
        if (!moving)
        {
            transform.position = BomGridRules.ToCell(transform.position);
            return;
        }

        Vector3 targetCell = settledCell + moveDirection;
        if (isCellBlocked != null && isCellBlocked(targetCell))
        {
            ForceStopAt(settledCell);
            return;
        }

        float distance = moveSpeed * Mathf.Max(0f, deltaTime) * 2f;
        transform.position = Vector3.MoveTowards(transform.position, targetCell, distance);
        if ((transform.position - targetCell).sqrMagnitude <= 0.000001f)
        {
            transform.position = targetCell;
            settledCell = targetCell;
        }
    }

    public void ForceStop()
    {
        ForceStopAt(BomGridRules.ToCell(transform.position));
    }

    public void ForceStopAt(Vector3 safeCell)
    {
        settledCell = BomGridRules.ToCell(safeCell);
        transform.position = settledCell;
        moving = false;
    }
}

public class Bom_Base_MoveManager : MonoBehaviour
{
    private BomMover mover;
    private BomStatusData status;
    private Bom_Base_CollisionManager collisionManager;

    private void Awake()
    {
        collisionManager = gameObject.AddComponent<Bom_Base_CollisionManager>();
        mover = gameObject.AddComponent<BomMover>();
    }

    public void BomAttack(BomParameters bomParams)
    {
        status = new BomStatusData(bomParams);
        if (status.bomAttack == BOM_ATTACK.BOM_ATTACK_THROW)
        {
            mover.ReqMove(bomParams.direction, bomParams.iSpeed);
        }
    }

    public void BomKick(Vector3 direction, int speed)
    {
        if (status != null && status.bomKick)
        {
            mover.ReqMove(direction, speed);
        }
    }

    private void Update()
    {
        mover.Advance(Time.deltaTime, IsCellBlocked);
    }

    private bool IsCellBlocked(Vector3 position)
    {
        return Library_Base.IsPositionOutOfBounds(position) ||
               collisionManager.CheckForCollisionAtCell(position);
    }

    public void Explosion()
    {
        mover.ForceStop();
    }
}

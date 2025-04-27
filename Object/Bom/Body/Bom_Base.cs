
using System.Collections.Generic;
using UnityEngine;

public class Bom_Base : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    protected BlockCreateManager cField;

    private BrokenBlockManager cBrokenBlockManager;

    public int iExplosionNum;
    protected InstanceManager_Base cInsManager;
    protected Bom_Base_MoveManager moveManager;
    protected Bom_Base_MaterialHandler materialHandler;
    //protected Bom_Base_CollisionManager collisionManager;

    public bool bDel;

    void Awake(){
        AwakeCommon();
    }

    public void AwakeCommon(){
        GameObject gField = GameObject.Find("Field");
        cBrokenBlockManager = gField.GetComponent<BrokenBlockManager>();
        cField = gField.GetComponent<BlockCreateManager>();
        moveManager = gameObject.AddComponent<Bom_Base_MoveManager>();
        //Bom_Base_CollisionManager collisionManager = gameObject.AddComponent<Bom_Base_CollisionManager>();
        materialHandler = gameObject.AddComponent<Bom_Base_MaterialHandler>();
        //moveManager.SetCollisionManager(collisionManager);
    }

    void Start()
    {
        init();
        //DelayMethodを3秒後に呼び出す
        Invoke(nameof(Explosion), 3f);
    }

    protected virtual void init(){
        AddComponentInstanceManager();
        ExplosionPrefab = Resources.Load<GameObject>(materialHandler.GetExplotionString());
        cInsManager.SetPrefab(ExplosionPrefab);	

    }
    public void SetMoveDirection(Vector3 direction)
    {
        moveManager.BomKick(direction);
    }
    
    public void SetMaterialKind(string sParamMaterial){
        materialHandler.SetMaterialKind(sParamMaterial);
    }


    protected virtual void AddComponentInstanceManager(){}

    public void CancelInvokeAndCallExplosion()
    {
        //Debug.Log("CancelInvokeAndCallExplosion");
        CancelInvoke(nameof(Explosion));
        Explosion();
    }

    // Update is called once per frame

    protected virtual bool IsWall(Vector3 v3Temp){
        bool bRet = cField.IsAllWall(v3Temp);
        return bRet;
    }


    // ブロックが壊れているかどうかの判定をするかどうかを派生クラスで制御
    protected virtual bool ShouldCheckIsBroken()
    {
        return true; // デフォルトでは判定を行う
    }

    protected virtual bool IsExplosion(){
        return false;
    }
    protected virtual void Explosion(){}

    private void OnDestroy()
    {
        // Destroy時に登録したInvokeをすべてキャンセル
        CancelInvoke();
    }


    protected virtual void HandleExplosion(Vector3 initialPosition)
    {
        if (cInsManager == null)
            return;

        moveManager.Explosion();
        transform.position = initialPosition;

        if (DestroyExistingExplosion(initialPosition))
        {
            cInsManager.InstantiateInstancePool(initialPosition);
        }

        // 各方向に爆風を生成
        ExplodeInAllDirections(transform.position);

        // 自身を削除
        cInsManager.DestroyInstance(this.gameObject);
    }

    private void ExplodeInAllDirections(Vector3 origin)
    {
        // 各方向ごとの移動ベクトル（X負, X正, Z負, Z正）
        List<Vector3> directions = new List<Vector3>
        {
            new Vector3(-1, 0, 0), // X負方向
            new Vector3(1, 0, 0),  // X正方向
            new Vector3(0, 0, -1), // Z負方向
            new Vector3(0, 0, 1)   // Z正方向
        };

        foreach (Vector3 dir in directions)
        {
            for (int i = 1; i <= iExplosionNum; i++)
            {
                Vector3 targetPos = origin + dir * i;
                if (CreateExplosionAndCheckContinuation(targetPos) == ExplosionResult.Stop)
                {
                    break;
                }
            }
        }
    }

/*
    protected virtual void HandleExplosion(Vector3 initialPosition)
    {
        if (cInsManager == null)
        {
            return;
        }

        moveManager.StopMoving();

        // 初期位置に移動
        transform.position = initialPosition;
        // 爆風のインスタンスを生成
        if(DestroyExistingExplosion(initialPosition)){
            cInsManager.InstantiateInstancePool(initialPosition);
        }
        // X方向の爆風を生成
        for (int i = 1; i <= iExplosionNum; i++)
        {
            Vector3 xNegativeDirection = new Vector3(transform.position.x - i, transform.position.y, transform.position.z); // X方向の負の方向
            if (ExplosionResult.Stop == CreateExplosionAndCheckContinuation(xNegativeDirection)) break; // X方向の負の方向
        }

        for (int i = 1; i <= iExplosionNum; i++)
        {
            Vector3 xPositiveDirection = new Vector3(transform.position.x + i, transform.position.y, transform.position.z); // X方向の正の方向
            if (ExplosionResult.Stop == CreateExplosionAndCheckContinuation(xPositiveDirection)) break; // X方向の正の方向
        }

        // Z方向の爆風を生成
        for (int i = 1; i <= iExplosionNum; i++)
        {
            Vector3 zNegativeDirection = new Vector3(transform.position.x, transform.position.y, transform.position.z - i); // Z方向の負の方向
            if (ExplosionResult.Stop == CreateExplosionAndCheckContinuation(zNegativeDirection)) break; // Z方向の負の方向
        }

        for (int i = 1; i <= iExplosionNum; i++)
        {
            Vector3 zPositiveDirection = new Vector3(transform.position.x, transform.position.y, transform.position.z + i); // Z方向の正の方向
            if (ExplosionResult.Stop == CreateExplosionAndCheckContinuation(zPositiveDirection)) break; // Z方向の正の方向
        }

        // このオブジェクトの破棄
        cInsManager.DestroyInstance(this.gameObject);
    }
*/


    public enum ExplosionResult
    {
        Continue,
        Stop
    }

    protected virtual ExplosionResult CreateExplosionAndCheckContinuation(Vector3 explosionPosition)
    {
        // 爆風生成の異常チェック（壁の判定、既存爆風の破棄）
        if (!IsExplosionCreationValid(explosionPosition))
        {
            return ExplosionResult.Stop;
        }

        if(DestroyExistingExplosion(explosionPosition)){
            // 爆風生成処理（失敗時に Stop を返す）
            if (!CreateExplosion(explosionPosition))
            {
                return ExplosionResult.Stop;
            }
        }
        // 継続判定
        return CanContinueExplosion(explosionPosition) ? ExplosionResult.Continue : ExplosionResult.Stop;
    }

    /// <summary>
    /// 爆風生成が有効かどうかをチェックする
    /// 壁や既存爆風の確認を行い、生成が無効な場合は false を返す
    /// </summary>
    protected bool IsExplosionCreationValid(Vector3 position)
    {
        if (IsWall(position))
        {
            return false;
        }

        return true;
    }

    protected bool DestroyExistingExplosion(Vector3 position)
    {
        GameObject existingExplosion = Library_Base.GetGameObjectAtExactPositionWithName(position, "Explosion");
        if (existingExplosion != null)
        {
            cInsManager.DestroyInstancePool(existingExplosion);
        }
        return true;
    }


    /// <summary>
    /// 爆風を生成し、成功したかどうかを返す
    /// </summary>
    protected virtual bool CreateExplosion(Vector3 position)
    {
        cInsManager.InstantiateInstancePool(position);
        return true;
    }

    /// <summary>
    /// 爆風を継続するかどうかを判定する
    /// ブロックが破壊可能な場合は継続を止める
    /// </summary>
    protected virtual bool CanContinueExplosion(Vector3 position)
    {
        if(ShouldCheckIsBroken() && cBrokenBlockManager.IsBroken(position)){
            return false;
        }
        return true;
    }
/*
    public void AbailableBomKick()
    {
        moveManager.AbailableBomKick();
    }

    public void AbailableBomAttack(Vector3 direction)
    {
        moveManager.AbailableBomAttack(direction);
    }
*/
}

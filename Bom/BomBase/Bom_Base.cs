using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;

public class Bom_Base : MonoBehaviourPunCallbacks
{
    public GameObject ExplosionPrefab;

    protected Material cMaterialType;
    public string sMaterialKind;
    protected string sExplosion;
    protected Field_Block_Base cField;

    protected Library_Base cLibrary;

    protected MaterialManager cMaterialMng;

    //protected BomControl cBomControl;
    //private List<GameObject> ExplosionList = new List<GameObject>();
    // Start is called before the first frame update
    private Vector3 moveDirection = Vector3.zero;
    //private Vector3 previousPosition;
    //private float moveSpeed = 1.5f; // 動く速さ
    //protected bool isMoving = false; // 移動中フラグ
    public int iExplosionNum;
    //protected object lockObject = new object(); // ロックオブジェクト
    protected InstanceManager_Base cInsManager;

    protected PhotonView cPhotonView;
    // Playerクラスから方向を受け取るメソッド

    protected Bom_Base_MoveManager moveManager;
    protected Bom_Base_ExplosionManager explosionManager;
    protected Bom_Base_MaterialHandler materialHandler;
    protected Bom_Base_CollisionManager collisionManager;

    public bool bDel;

    public void SetMoveDirection(Vector3 direction)
    {
        //moveDirection =direction;
        moveManager.SetMoveDirection(direction);
    }
    
    public void SetMaterialKind(string sParamMaterial){
        sMaterialKind = sParamMaterial;
    }

    void Awake(){
        cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
        cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        cPhotonView = GetComponent<PhotonView>();
        moveManager = gameObject.AddComponent<Bom_Base_MoveManager>();
    }

    protected virtual void init(){
        //cInsManager = gameObject.AddComponent<InstanceManager_Base>();
        AddComponentInstanceManager();
        sExplosion = cMaterialMng.GetMaterialOfExplosion(sMaterialKind);
        ExplosionPrefab = Resources.Load<GameObject>(sExplosion);
        cInsManager.SetPrefab(ExplosionPrefab);	

    }

    protected virtual void AddComponentInstanceManager(){}

    void Start()
    {
        // インスタンス生成直後にマテリアルを設定している事もあり、Awakeのタイミングではまだマテリアルが取得できない。
        // 初回描画のタイミングであれば取得可能であるため、ここでマテリアルを設定している
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        if (renderer != null)
        {
            MaterialManager materialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
            Material newMaterial = materialManager.GetMaterialOfType(sMaterialKind);
            renderer.material = newMaterial;
        }
        else
        {
            Debug.LogWarning("Renderer component not found on the game object or its children.");
        }
        init();
        //DelayMethodを3秒後に呼び出す
        Invoke(nameof(Explosion), 3f);
    }

    public void CancelInvokeAndCallExplosion()
    {
        //Debug.Log("CancelInvokeAndCallExplosion");
        CancelInvoke(nameof(Explosion));
        Explosion();
    }

    // Update is called once per frame
    void Update()
    {
/*
        if (isMoving)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime * 2;
            CheckForCollision();
        }
*/
        moveManager.Move(transform);
    }

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

    protected void HandleExplosion(Vector3 initialPosition)
    {
        if (cInsManager == null)
        {
            return;
        }

        moveManager.StopMoving();

        // 初期位置に移動
        transform.position = initialPosition;

        // 既存の爆風がある場合は破棄
        GameObject existingExplosion = Library_Base.IsPositionAndName(initialPosition, "Explosion");
        if (existingExplosion != null)
        {
            cInsManager.DestroyInstancePool(existingExplosion);
        }

        // 爆風のインスタンスを生成
        cInsManager.InstantiateInstancePool(initialPosition);
        /*
        GameObject initialExplosion = cInsManager.InstantiateInstancePool(initialPosition);
        if (initialExplosion == null)
        {
            return; // インスタンス生成失敗時は処理を終了
        }
        initialExplosion.transform.position = initialPosition;
        */

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

        DestroyExistingExplosion(explosionPosition);

        // 爆風生成処理（失敗時に Stop を返す）
        if (!CreateExplosion(explosionPosition))
        {
            return ExplosionResult.Stop;
        }

        // 継続判定
        return CanContinueExplosion(explosionPosition) ? ExplosionResult.Continue : ExplosionResult.Stop;
    }

    /// <summary>
    /// 爆風生成が有効かどうかをチェックする
    /// 壁や既存爆風の確認を行い、生成が無効な場合は false を返す
    /// </summary>
    protected virtual bool IsExplosionCreationValid(Vector3 position)
    {
        if (IsWall(position))
        {
            return false;
        }

        return true;
    }

    private void DestroyExistingExplosion(Vector3 position)
    {
        GameObject existingExplosion = Library_Base.IsPositionAndName(position, "Explosion");
        if (existingExplosion != null)
        {
            cInsManager.DestroyInstancePool(existingExplosion);
        }
    }
    /// <summary>
    /// 爆風を生成し、成功したかどうかを返す
    /// </summary>
    protected virtual bool CreateExplosion(Vector3 position)
    {
        cInsManager.InstantiateInstancePool(position);
        return true;
        /*
        GameObject newExplosion = cInsManager.InstantiateInstancePool(position);
        if (newExplosion == null)
        {
            return false;
        }

        newExplosion.transform.position = position;
        return true;
        */
    }

    /// <summary>
    /// 爆風を継続するかどうかを判定する
    /// ブロックが破壊可能な場合は継続を止める
    /// </summary>
    protected virtual bool CanContinueExplosion(Vector3 position)
    {
        if(ShouldCheckIsBroken() && cField.IsBroken(position)){
            return false;
        }
        return true;
    }

/*
    //update関数の中でCheckForCollisionをコールして衝突検知する。
    //OnCollisionEnterでの検知では、タイミングが遅く、オブジェクトの中に入り込んだときに検知してしまい、手前で止まらない。
    void CheckForCollision()
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
                    isMoving = false; // 移動停止
                    break;
                default:
                    // 上記の条件に該当しない場合は何もしない
                    return;
            }
        }
    }
*/
    void OnTriggerEnter(Collider other)
    {
        switch (other.transform.name)
        {
            case "Explosion1(Clone)":
            case "Explosion2(Clone)":
            case "Explosion3(Clone)":
            case "Explosion4(Clone)":
                CancelInvokeAndCallExplosion();
                break;
            default:
                return;
        }
    }


    void OnTriggerExit(Collider other)
    {
        //Debug.Log("すり抜けた！");
        GetComponent<SphereCollider>().isTrigger = false;
        
    }
/*
    public void AbailableBomKick(){
        isMoving = true;
    }

    public void AbailableBomAttack(Vector3 direction){
        isMoving = true;
        SetMoveDirection(direction);
    }
*/
    public void AbailableBomKick()
    {
        moveManager.AbailableBomKick();
    }

    public void AbailableBomAttack(Vector3 direction)
    {
        moveManager.AbailableBomAttack(direction);
    }
}

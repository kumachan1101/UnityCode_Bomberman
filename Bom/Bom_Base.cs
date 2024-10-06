using UnityEngine;
using Photon.Pun;

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
    private float moveSpeed = 1.5f; // 動く速さ
    protected bool isMoving = false; // 移動中フラグ
    public int iExplosionNum;
    protected object lockObject = new object(); // ロックオブジェクト
    protected InstanceManager_Base cInsManager;

    protected PhotonView cPhotonView;
    // Playerクラスから方向を受け取るメソッド
    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection =direction;
        //moveDirection = direction.normalized; // 方向を正規化することで、速度を一定に保つ
    }
    
    public void SetMaterialKind(string sParamMaterial){
        sMaterialKind = sParamMaterial;
    }

    void Awake(){
        cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
        cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        cPhotonView = GetComponent<PhotonView>();
    }

    protected virtual void init(){
        cInsManager = gameObject.AddComponent<InstanceManager_Base>();
        sExplosion = cMaterialMng.GetMaterialOfExplosion(sMaterialKind);
        ExplosionPrefab = Resources.Load<GameObject>(sExplosion);
        cInsManager.SetPrefab(ExplosionPrefab);		
    }

    // Start is called before the first frame update
    void Start()
    {
        //previousPosition = transform.position;
        // インスタンス生成直後にマテリアルを設定している事もあり、Awakeのタイミングではまだマテリアルが取得できない。
        // 初回描画のタイミングであれば取得可能であるため、ここでマテリアルを設定している
        //GetComponent<Renderer>().material = cMaterialMng.GetMaterialOfType(sMaterialKind);
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
        if(GameManager.xmax <= transform.position.x || GameManager.zmax <= transform.position.z || 0 > transform.position.x || 0 > transform.position.z){
            return;
        }

        if (isMoving)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime * 2;
            CheckForCollision();
        }
    }

    protected virtual bool IsWall(Vector3 v3Temp){
        bool bRet = cField.IsAllWall(v3Temp);
        return bRet;
    }
    protected virtual bool XorZ_Explosion(Vector3 v3Temp)
    {
        // 壁の判定
        bool bRet = IsWall(v3Temp);
        if (bRet)
        {
            return false;
        }

        // 既存の爆発を破棄
        GameObject gExplosion = cLibrary.IsPositionAndName(v3Temp, "Explosion");
        if (gExplosion != null)
        {
            cInsManager.DestroyInstancePool(gExplosion);
        }

        // 新しい爆発のインスタンスを生成
        GameObject g = cInsManager.InstantiateInstancePool(v3Temp);
        if (g == null)
        {
            return false;
        }

        // ブロックが破壊可能かどうかを派生クラスで判定する
        if (ShouldCheckIsBroken() && cField.IsBroken(v3Temp))
        {
            g.transform.position = v3Temp;
            return false;
        }

        g.transform.position = v3Temp;
        return true;
    }

    // ブロックが壊れているかどうかの判定をするかどうかを派生クラスで制御
    protected virtual bool ShouldCheckIsBroken()
    {
        return true; // デフォルトでは判定を行う
    }

    protected virtual bool X_Explosion(int i){
        Vector3 v3Temp = new Vector3(transform.position.x+i,transform.position.y,transform.position.z);
        bool bRet = XorZ_Explosion(v3Temp);
        return bRet;
    }

    protected virtual bool Z_Explosion(int i){
        Vector3 v3Temp = new Vector3(transform.position.x,transform.position.y,transform.position.z+i);
        bool bRet = XorZ_Explosion(v3Temp);
        return bRet;
    }

    protected virtual bool IsExplosion(){
        return false;
    }
    protected virtual void Explosion(){}

    protected void HandleExplosion(Vector3 v3)
    {
        transform.position = v3;
        GameObject gExplosion = cLibrary.IsPositionAndName(v3, "Explosion");
        if (gExplosion != null)
        {
            cInsManager.DestroyInstancePool(gExplosion);
        }

        GameObject g = cInsManager.InstantiateInstancePool(v3);
        if (g == null)
        {
            return;
        }
        g.transform.position = v3;

        for (int i = 1; i <= iExplosionNum; i++)
        {
            if (!X_Explosion(i * (-1))) break;
        }

        for (int i = 1; i <= iExplosionNum; i++)
        {
            if (!X_Explosion(i)) break;
        }

        for (int i = 1; i <= iExplosionNum; i++)
        {
            if (!Z_Explosion(i * (-1))) break;
        }

        for (int i = 1; i <= iExplosionNum; i++)
        {
            if (!Z_Explosion(i)) break;
        }

        cInsManager.DestroyInstance(this.gameObject);
    }


    private void OnDestroy()
    {
        // Destroy時に登録したInvokeをすべてキャンセル
        CancelInvoke();
    }

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

    public void AbailableBomKick(){
        isMoving = true;
    }

    public void AbailableBomAttack(Vector3 direction){
        isMoving = true;
        SetMoveDirection(direction);
    }


}

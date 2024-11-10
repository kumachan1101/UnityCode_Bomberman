using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Field_Block_Base:MonoBehaviourPunCallbacks 
{
    public GameObject FixedWallPrefab;
    public GameObject GroundPrefab;
    public GameObject BrokenPrefab;

    public GameObject GroundExplosionPrefab;

    public GameObject ObjMovePrefab;

    public List<GameObject> GroundList = new List<GameObject>();
    public List<GameObject> WallList = new List<GameObject>();
    public List<GameObject> FixedWallList = new List<GameObject>();
    public List<GameObject> BrokenList = new List<GameObject>();

    public List<GameObject> ObjMoveList = new List<GameObject>();

    protected Library_Base cLibrary;
    protected ExplosionManager explosionManager;

    private object lockObject = new object(); // ロックオブジェクト

    protected virtual void ClearBrokenList_RPC() { }
    protected virtual void InsBrokenBlock_RPC(int x, int y, int z) { }

    protected virtual void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection) { }

    public virtual void Rainbow_RPC(string sMaterialType) { }

    [SerializeField] protected ObjectPooler_Base objectPooler;


    void Start()
    {
        SetupStage();

        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
        CreateFixedBlock();
        CreateField();

    }

    public void SetupStage()
    {
        objectPooler.pools.Clear();
        ConfigurePools();
        objectPooler.InitializePool(); // プールを再生成
    }

    virtual protected void ConfigurePools()
    {
        explosionManager.AddPool(ExplosionTypes.Explosion1, 5000);
        explosionManager.AddPool(ExplosionTypes.Explosion2, 5000);
        // 他のExplosionタイプが必要であれば追加
    }


    void Awake()
    {
        FixedWallPrefab = Resources.Load<GameObject>("FixedWall");
        GroundPrefab = Resources.Load<GameObject>("Ground");
        BrokenPrefab = Resources.Load<GameObject>("Broken");
        GroundExplosionPrefab = Resources.Load<GameObject>("Explosion1");
        ObjMovePrefab = Resources.Load<GameObject>("ObjMove");

        objectPooler = GetComponent<ObjectPooler_Base>();
        explosionManager = new GameObject("ExplosionManager").AddComponent<ExplosionManager>();
        explosionManager.Initialize(objectPooler, cLibrary);

    }

    public void CreateField()
    {
        AddBrokenBlock(5);
    }

    [PunRPC]
    public void ClearBrokenList()
    {
        foreach (GameObject gBroken in BrokenList)
        {
            if (gBroken != null)
            {
                Destroy(gBroken);
            }
        }
        BrokenList.Clear();
    }

    [PunRPC]
    public void InsBrokenBlock(int x, int y, int z)
    {
        GameObject g = Instantiate(BrokenPrefab);
        g.transform.position = new Vector3(x, y, z);
        BrokenList.Add(g);
    }

    public void AddBrokenBlock(int randomRangeMax)
    {
        StartCoroutine(AddBrokenBlockCoroutine(randomRangeMax));
    }

    private IEnumerator AddBrokenBlockCoroutine(int randomRangeMax)
    {
        int y = 1;
        for (int x = 0; x < GameManager.xmax; x++)
        {
            for (int z = 0; z < GameManager.zmax; z++)
            {
                Vector3 v3 = new Vector3(x, y, z);
                if (false == Library_Base.IsGameObjectAtPosition(v3))
                {
                    int iRand = Random.Range(0, randomRangeMax);
                    if (0 == iRand)
                    {
                        InsBrokenBlock_RPC(x, y, z);
                    }
                }
                if (z % 10 == 0)
                {
                    yield return null;
                }
            }
            if (x % 10 == 0)
            {
                yield return null;
            }
        }
    }

    [PunRPC]
    public void InsObjMove(int x, int y, int z, Library_Base.Direction randomDirection)
    {
        ObjMovePrefab = Resources.Load<GameObject>("ObjMove");
        GameObject g = Instantiate(ObjMovePrefab);
        g.transform.position = new Vector3(x, y, z);

        Library_Base.SetDirection(g, randomDirection);

        ObjMoveList.Add(g);
    }

    protected virtual void SetFieldRange()
    {
        GameManager.SetFieldRange(20, 20);
    }

    protected void CreateFixedBlock(){
		SetFieldRange();
		GetComponent<Field_Player_Base>().SetPlayerPositions();
		
        for (int x = 0; x < GameManager.xmax; x++) {
            for (int z = 0; z < GameManager.zmax; z++) {
                //int iRand = Random.Range(0, 10);
                int iRand = 0;
                int y1 = 0;
                int y2 = 1;

                //矢印ブロックは生成しないようにしている。オンラインの場合、このタイミングで生成しようとすると、Pothon実行前と思われ、エラーになってしまうため。
                //オンラインに対応するのであれば、破壊ブロックと同じタイミングでPothonから生成するようにする。
                if(1 == iRand){
                    Library_Base.Direction randomDirection = Library_Base.GetRandomDirection();
                    InsObjMove_RPC(x, y1, z, randomDirection);
                }
                else{
                    //Instantiateはprehabの抽象データBoxを複製して実体化する
                    GameObject g1 = Instantiate(GroundPrefab);
                    g1.transform.position = new Vector3(x, y1, z);
                    GroundList.Add(g1);
                }

                
                if((x == 0 || z == 0 || x == GameManager.xmax-1 || z == GameManager.zmax-1)
				|| (x == 1 || z == 1 || x == GameManager.xmax-2 || z == GameManager.zmax-2)){
                    GameObject g2 = Instantiate(FixedWallPrefab);
                    g2.transform.position = new Vector3(x, y2, z);
                    FixedWallList.Add(g2);
                }

            }
        }


    }

    public bool IsAllWall(Vector3 v3){
        bool isWall = cLibrary.IsObjectAtPosition(WallList, v3);
        bool isFixedWall = cLibrary.IsObjectAtPosition(FixedWallList, v3);
        bool isAllWall = isWall | isFixedWall;
        return isAllWall;
    }

    public bool IsBroken(Vector3 v3){
        bool isBroken = cLibrary.IsObjectAtPosition(BrokenList, v3);
        return isBroken;
    }



	public bool IsMatchObjMove(Vector3 targetPosition)
	{
		foreach (GameObject obj in ObjMoveList)
		{
			if (obj != null && obj.transform.position == targetPosition)
			{
				return true; // 位置とマテリアルが一致する場合
			}
		}
		return false; // 位置とマテリアルが一致しない場合
	}

    public bool IsMatch(Vector3 targetPosition, Material targetMaterial)
    {
        return explosionManager.IsMatch(targetPosition, targetMaterial);
    }


    public void UpdateGroundExplosion(GameObject gObj)
    {
        explosionManager.UpdateGroundExplosion(gObj);
    }

    public GameObject DequeueObect(string tag)
    {
        return explosionManager.DequeueObject(tag);
    }

    public void EnqueueObject(GameObject obj)
    {
		explosionManager.EnqueueObject(obj);
    }


    [PunRPC]
    public void Rainbow(string sMaterialType)
    {
        MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        string StrExplosion = cMaterialMng.GetMaterialOfExplosion(sMaterialType);

        foreach (GameObject obj in explosionManager.ExplosionList)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material newMaterial = cMaterialMng.GetMaterialOfTypeExplosion(StrExplosion);
                    renderer.material = newMaterial;
                }

                Explosion_Base cExplosion = obj.GetComponent<Explosion_Base>();
                //cExplosion.SetPosition(obj.transform.position);
            }
        }
    }
}

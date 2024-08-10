using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;
public class Field_Block_Base : Field_Base{
    public GameObject FixedWallPrefab;
    public GameObject GroundPrefab;
    public GameObject BrokenPrefab;

    public GameObject GroundExplosionPrefab;

    public GameObject ObjMovePrefab;

    public List<GameObject> GroundList = new List<GameObject>();
    public List<GameObject> WallList = new List<GameObject>();
    public List<GameObject> FixedWallList = new List<GameObject>();
    public List<GameObject> BrokenList = new List<GameObject>();

    public List<GameObject> ExplosionList = new List<GameObject>();

    public List<GameObject> ObjMoveList = new List<GameObject>();

    protected Library_Base cLibrary;

    private object lockObject = new object(); // ロックオブジェクト

	protected virtual void ClearBrokenList_RPC(){}
    protected virtual void InsBrokenBlock_RPC(int x, int y, int z){}

    protected virtual void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){}

    protected virtual void Rainbow_RPC(string sMaterialType){}

    [SerializeField]protected ObjectPooler_Base objectPooler;

    void Start()
    {
		objectPooler = GetComponent<ObjectPooler_Base>();
		SetupStage();

        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
		CreateFixedBlock();
    }


    public void SetupStage()
    {
        objectPooler.pools.Clear();
        ConfigurePools();
        objectPooler.InitializePool(); // プールを再生成
    }

    protected virtual void ConfigurePools(){
        AddPool(ExplosionTypes.Explosion1, 1000);
        AddPool(ExplosionTypes.Explosion2, 1000);
	}

    protected void AddPool(string tag, int size)
    {
        GameObject prefab = Resources.Load<GameObject>(tag);
        ObjectPooler_Base.Pool newPool = new ObjectPooler_Base.Pool { tag = tag, prefab = prefab, size = size };
        objectPooler.pools.Add(newPool);
    }


    // DequeueObject メソッドを公開
    public GameObject DequeueObject(string tag)
    {
        return objectPooler.DequeueObject(tag);
    }

    // EnqueueObject メソッドを公開
    public void EnqueueObject(GameObject obj)
    {
		string tag = GetExplosionType(obj.name);
        objectPooler.EnqueueObject(tag, obj);
    }

    void Awake(){
        FixedWallPrefab = Resources.Load<GameObject>("FixedWall");
		GroundPrefab = Resources.Load<GameObject>("Ground");
		BrokenPrefab = Resources.Load<GameObject>("Broken");
		GroundExplosionPrefab = Resources.Load<GameObject>("Explosion1");
		ObjMovePrefab = Resources.Load<GameObject>("ObjMove");
    }

	public void CreateField(){
        
		//CreateBrokenBlock();
		AddBrokenBlock(5);
	}

    [PunRPC]
    public void ClearBrokenList(){
        foreach (GameObject gBroken in BrokenList) {
            if(null != gBroken){
                Destroy(gBroken);
            }
        }
        BrokenList.Clear();
    }


    [PunRPC]
    public void InsBrokenBlock(int x, int y, int z){
        GameObject g = Instantiate(BrokenPrefab);
        g.transform.position = new Vector3(x, y, z);
        BrokenList.Add(g);
    }


	public void AddBrokenBlock(int randomRangeMax) {
		StartCoroutine(AddBrokenBlockCoroutine(randomRangeMax));
	}

	private IEnumerator AddBrokenBlockCoroutine(int randomRangeMax) {
		int y = 1;
		for (int x = 0; x < GameManager.xmax; x++) {
			for (int z = 0; z < GameManager.zmax; z++) {
				Vector3 v3 = new Vector3(x, y, z);
				//if (!Library_Base.CheckPosition(v3)) {
				if( false == Library_Base.IsGameObjectAtPosition(v3)){
					int iRand = Random.Range(0, randomRangeMax);
					if (0 == iRand) {
						InsBrokenBlock_RPC(x, y, z);
					}
				}
				// 一定数の処理ごとにフレームを待機
				if (z % 10 == 0) {
					yield return null;
				}
			}
			// 一定数の処理ごとにフレームを待機
			if (x % 10 == 0) {
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

        // 向きを変更し、移動方向を設定する
        Library_Base.SetDirection(g, randomDirection);

        ObjMoveList.Add(g);
    }

	protected virtual void SetFieldRange(){
		GameManager.SetFieldRange(20,20);
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

    public void SetBrokenTrriger(bool bSet){
        foreach (GameObject obj in BrokenList) {
            if(obj != null){
                obj.GetComponent<Broken>().SetIsTrigger(bSet);
            }
        }
    }

	protected virtual void DestroySync(GameObject g){
		if (g == null)
		{
			Debug.LogWarning("Instance is null, cannot enqueue.");
			return;
		}
		EnqueueObject(g);
	}


    public void UpdateGroundExplosion(GameObject gObj)
    {
        lock (lockObject)
        {
            Material cMaterial = gObj.GetComponent<Renderer>().material;
            GameObject objToRemove = null;

            foreach (GameObject obj in ExplosionList)
            {
                if (obj != null && obj.transform.position == gObj.transform.position)
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer.material.name == cMaterial.name)
                    {
						DestroySync(gObj);
                        return;
                    }
                    else
                    {
                        //Debug.Log("renderer.material.name: " + renderer.material.name + " cMaterial.name: " + cMaterial.name);
                        objToRemove = obj;
                        break;
                    }
                }
            }

            if (objToRemove != null)
            {
                ExplosionList.Remove(objToRemove);
				DestroySync(objToRemove);
            }

            ExplosionList.Add(gObj);
        }
    }

    // 引数として受け取ったVector3とMaterialが、ExplosionList内のGameObjectの位置とマテリアルと一致するかどうかを判定する関数
    public bool IsMatch(Vector3 targetPosition, Material targetMaterial)
    {
        //Debug.Log(ExplosionList.Count);
        foreach (GameObject obj in ExplosionList)
        {
            //Debug.Log("obj.transform.position: " + obj.transform.position + "targetPosition : " + targetPosition);
            if (obj != null && obj.transform.position == targetPosition)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                //Debug.Log("renderer.material.name: " + renderer.material.name + "targetMaterial.name : " + targetMaterial.name);
                if (renderer != null && renderer.material.name.Replace(" (Instance)", "") == targetMaterial.name)
                {
                    return true; // 位置とマテリアルが一致する場合
                }
				else{
					return false;
				}
            }
        }
        return true;
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



    public void DelExplosion(Vector3 v3){
        GameObject delobj = null;
        foreach (GameObject obj in ExplosionList) {
            if(obj != null){
                if(obj.transform.position == v3){
                    delobj = obj;
                    break;
                }
            }
        }
        if(null != delobj){
            ExplosionList.Remove(delobj);
            Destroy(delobj);
        }
    }

    private bool IsExplosion(Vector3 v3){
        bool isExplosion = cLibrary.IsObjectAtPosition(ExplosionList, v3);
        return isExplosion;
    }

    private bool IsWall(Vector3 v3){
        bool isWall = cLibrary.IsObjectAtPosition(WallList, v3);
        return isWall;
    }

    private bool IsFixedWall(Vector3 v3){
        bool isFixedWall = cLibrary.IsObjectAtPosition(FixedWallList, v3);
        return isFixedWall;
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

    public void AddExplosion(GameObject g){
        ExplosionList.Add(g);       
    }

    public string GetExplosionType(string input)
    {
        if (input.Contains(ExplosionTypes.Explosion1))
        {
            return ExplosionTypes.Explosion1;
        }
        else if (input.Contains(ExplosionTypes.Explosion2))
        {
            return ExplosionTypes.Explosion2;
        }
        else if (input.Contains(ExplosionTypes.Explosion3))
        {
            return ExplosionTypes.Explosion3;
        }
        else if (input.Contains(ExplosionTypes.Explosion4))
        {
            return ExplosionTypes.Explosion4;
        }
        else
        {
            return null;
        }
    }

   [PunRPC]
    public void Rainbow(string sMaterialType){

        MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        string StrExplosion = cMaterialMng.GetMaterialOfExplosion(sMaterialType);

        List<GameObject> objectsToRemove = new List<GameObject>();
        List<GameObject> newExplosionList = new List<GameObject>(); // 新しいリストを作成

        foreach (GameObject obj in ExplosionList)
        {
            if (obj != null)
            {
                Vector3 v3 = obj.transform.position;
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material newMaterial = cMaterialMng.GetMaterialOfTypeExplosion(StrExplosion);
                    renderer.material = newMaterial;
                }

				Explosion_Base cExplosion = obj.GetComponent<Explosion_Base>();
				cExplosion.SetPosition(v3);
            }
        }

    }

}
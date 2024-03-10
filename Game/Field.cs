using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Field : MonoBehaviourPunCallbacks {

    public GameObject WallPrefab;
    public GameObject FixedWallPrefab;
    public GameObject GroundPrefab;
    public GameObject BrokenPrefab;

    public GameObject GroundExplosionPrefab;

    public List<GameObject> GroundList = new List<GameObject>();
    public List<GameObject> WallList = new List<GameObject>();
    public List<GameObject> FixedWallList = new List<GameObject>();
    public List<GameObject> BrokenList = new List<GameObject>();

    public List<GameObject> ExplosionList = new List<GameObject>();

    private int xmax;
    private int zmax;
    protected string PlayerName;

    private Vector3[] v3PlayerPos = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(20, 1, 20),
        new Vector3(1, 1, 20),
        new Vector3(20, 1, 1)
    };

    private object lockObject = new object(); // ロックオブジェクト

    void Awake() {
    }


    // Start is called before the first frame update
    void Start()
    {
        /*
        int iPlayerCnt = GameObject.Find("Online").GetComponent<Online>().GetPlayerCnt();
        if(iPlayerCnt >= 2){
            return;
        }
        */
        xmax = 22;
        zmax = 22;
        CreateBox();
        CPUmodeInit();
    }

    protected virtual void CPUmodeInit(){
    }

    protected void SpawnPlayerObjects(int playerCount)
    {
        for (int i = 1; i <= playerCount; i++)
        {
            string canvasName = "Canvas" + i;
            GameObject gCanvas = LoadResource(canvasName);
            Vector3 v3PwrGage = new Vector3(0, 0, 0);
            gCanvas.transform.position = v3PwrGage;

            Vector3 v3PlayerPos = GetPlayerPosition(i);
            string PlayerName = "Player" + i;
            GameObject gPlayer = LoadResource(PlayerName);
            gPlayer.transform.position = v3PlayerPos;
            Player cPlayer;
            if(i == 1){
                cPlayer = gPlayer.GetComponent<Player>();
            }
            else{
                cPlayer = gPlayer.GetComponent<Player_CpuMode>();
            }
            /*
            Debug.Log(cPlayer.name);
            Debug.Log(cPlayer);
            Component[] allComponents = gPlayer.GetComponents<Component>();

            // コンポーネントの情報をログに表示

            foreach (Component component in allComponents)
            {
                Debug.Log("Component: " + component.GetType().Name);
            }
            */
            //Debug.Log(cPlayer);
            cPlayer.SetViewID(i); // カウンタiを使用する
            cPlayer.SetSlider(gCanvas);
            //Debug.Log(gPlayer.name);
        }
    }

    private GameObject LoadResource(string loadname){
        // Resourcesフォルダ内のPlayer1プレハブを読み込む
        GameObject playerPrefab = Resources.Load<GameObject>(loadname);
        return Instantiate(playerPrefab);

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // playerCountを引数に取り、v3PlayerPos[playerCount-1]を返す関数
    public Vector3 GetPlayerPosition(int playerCount)
    {
        if (playerCount >= 1 && playerCount <= v3PlayerPos.Length)
        {
            return v3PlayerPos[playerCount - 1];
        }
        else
        {
            Debug.LogError("Invalid playerCount: " + playerCount);
            return Vector3.zero; // エラー時は適当な値を返すか、エラーハンドリングを行う
        }
    }

    public void CreateBrokenBlock(){
        ClearBrokenList_RPC();
        int y = 1;
        for (int x = 0; x < xmax; x++) {
            for (int z = 0; z < zmax; z++) {
                if(!IsAbalableBlock(x,z)){
                    break;
                }
                Vector3 v3 = new Vector3(x,y,z);
                if(false == IsWall(v3) && false == IsFixedWall(v3)){
                    int iRand = Random.Range(0, 5);
                    if(0 == iRand){
                        
                        InsBrokenBlock_RPC(x, y, z);
                    }
                }
            }
        }
    }



    private bool IsAbalableBlock(int x,int z){
        if ((x == 1 || x == 2 || x == xmax-3 || x == xmax-2) && (z == zmax-3 || z == zmax-2 || z == 1 || z == 2))
        {
            return false;
        }
        return true;
    }

    protected virtual void ClearBrokenList_RPC(){
        photonView.RPC(nameof(ClearBrokenList), RpcTarget.All);
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
    protected virtual void InsBrokenBlock_RPC(int x, int y, int z){
        photonView.RPC(nameof(InsBrokenBlock), RpcTarget.All, x, y, z);
    }


    [PunRPC]
    public void InsBrokenBlock(int x, int y, int z){
        GameObject g = Instantiate(BrokenPrefab);
        g.transform.position = new Vector3(x, y, z);
        BrokenList.Add(g);
    }

    private void CreateExplode(Vector3 v3){
        GameObject g = Instantiate(GroundExplosionPrefab);            
        string sMaterialType = GetBomMaterial((int)v3.x, (int)v3.z);
        if(sMaterialType == "InvalidMaterial"){
            Destroy(g);
            return;
        }
        MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        Material newMaterial = cMaterialMng.GetMaterialOfType(sMaterialType);
        g.GetComponent<Renderer>().material = newMaterial;

        g.GetComponent<Explosion>().FieldValid();
        g.transform.position = v3;
        AddExplosion(g);
    }



    private string GetBomMaterial(int x, int z)
    {
        if ((x == 1 && z == 1) || (x == 1 && z == 2) || (x == 2 && z == 1) || (x == 2 && z == 2))
        {
            return "BomMaterial1";
        }
        else if ((x == xmax - 2 && z == zmax - 2) || (x == xmax - 2 && z == zmax - 3) || (x == xmax - 3 && z == zmax - 2) || (x == xmax - 3 && z == zmax - 3))
        {
            return "BomMaterial2";
        }
        else if ((x == 1 && z == zmax - 2) || (x == 1 && z == zmax - 3) || (x == 2 && z == zmax - 2) || (x == 2 && z == zmax - 3))
        {
            return "BomMaterial3";
        }
        else if ((x == xmax - 2 && z == 1) || (x == xmax - 2 && z == 2) || (x == xmax - 3 && z == 1) || (x == xmax - 3 && z == 2))
        {
            return "BomMaterial4";
        }
        else
        {
            // 対応する座標がない場合、もしくは不正な座標が渡された場合
            return "InvalidMaterial";
        }
    }
    private void CreateBox(){
        for (int x = 0; x < xmax; x++) {
            for (int z = 0; z < zmax; z++) {
                //Instantiateはprehabの抽象データBoxを複製して実体化する
                GameObject g1 = Instantiate(GroundPrefab);
                int y1 = 0;
                g1.transform.position = new Vector3(x, y1, z);
                GroundList.Add(g1);

                int y2 = 1;
                
                if(x == 0 || z == 0 || x == xmax-1 || z == zmax-1){
                    GameObject g2 = Instantiate(FixedWallPrefab);
                    g2.transform.position = new Vector3(x, y2, z);
                    FixedWallList.Add(g2);
                }
                CreateExplode(new Vector3(x, 0, z));
                /*
                else if(x % 2 == 0 && z % 2 == 0){
                    GameObject g2 = Instantiate(WallPrefab);
                    g2.transform.position = new Vector3(2*x, y2, 2*z);
                    WallList.Add(g2);
                }
                */
            }
        }


    }

    public int GetBoxNum(){
        int iLen = 0;
        foreach (GameObject gBox in WallList) {
            if(null != gBox){
                iLen++;
            }
        }
        return iLen;
    }



    public bool IsObjectAtPosition(List<GameObject> objectList, Vector3 v3){
        foreach (GameObject obj in objectList) {
            if(obj != null){
                if(obj.transform.position == v3){
                    return true;
                }
            }
        }
        return false;
    }



    public void SetBrokenTrriger(bool bSet){
        foreach (GameObject obj in BrokenList) {
            if(obj != null){
                obj.GetComponent<Broken>().SetIsTrigger(bSet);
            }
        }
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
                        Destroy(gObj);
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
                Destroy(objToRemove);
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
            }
        }
        return false; // 位置とマテリアルが一致しない場合
    }

    public bool IsExplosion(Vector3 v3){
        bool isExplosion = IsObjectAtPosition(ExplosionList, v3);
        return isExplosion;
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

    public bool IsWall(Vector3 v3){
        bool isWall = IsObjectAtPosition(WallList, v3);
        return isWall;
    }

    public bool IsFixedWall(Vector3 v3){
        bool isFixedWall = IsObjectAtPosition(FixedWallList, v3);
        return isFixedWall;
    }

    public bool IsAllWall(Vector3 v3){
        bool isWall = IsObjectAtPosition(WallList, v3);
        bool isFixedWall = IsObjectAtPosition(FixedWallList, v3);
        bool isAllWall = isWall | isFixedWall;
        return isAllWall;
    }


    public bool IsBroken(Vector3 v3){
        bool isBroken = IsObjectAtPosition(BrokenList, v3);
        return isBroken;
    }

    public void AddExplosion(GameObject g){
        ExplosionList.Add(g);       
    }

    // 指定した座標と名称が一致し、かつアクティブであるオブジェクトが存在するかをチェックする関数
    public bool CheckPositionAndName(Vector3 targetPosition, string targetName)
    {
        // シーン内の全てのGameObjectを取得
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        // 全てのGameObjectに対してループ処理を行う
        foreach (GameObject obj in allGameObjects)
        {
            // GameObjectのTransformコンポーネントを取得
            Transform objTransform = obj.transform;

            // アクティブかつ座標と名称が一致するかをチェックする
            if (obj.activeInHierarchy && objTransform.position == targetPosition && obj.name == targetName)
            {
                // 一致する場合はtrueを返す
                return true;
            }
        }

        // 一致するGameObjectが見つからない場合はfalseを返す
        return false;
 
 
    }


    public void SetName(string namepara){
        PlayerName = namepara;
    }

    public string GetName(){
        return PlayerName;
    }


}
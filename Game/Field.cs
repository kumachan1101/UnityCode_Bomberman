using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;

public class Field : MonoBehaviourPunCallbacks {

    public GameObject WallPrefab;
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


    protected string PlayerName;

    protected Vector3[] v3PlayerPos1 = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, 1)
    };

    private object lockObject = new object(); // ロックオブジェクト

    private int CuurentPlayerNum; //プレイヤー自身を除く他プレイヤーの数
    protected int m_playerCount; //やられたプレイヤー含む全プレイヤー数

    //private int iPowerGageYPos;

    void Awake() {
    }


    public Vector3 GetPlayerPosition(int arrayIndex, int elementIndex)
    {
        // 変数名を構築
        string variableName = "v3PlayerPos" + arrayIndex;

        // フィールドを取得して値を取得
        FieldInfo field = GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            Vector3[] array = (Vector3[])field.GetValue(this);
            if (elementIndex >= 0 && elementIndex < array.Length)
            {
                return array[elementIndex];
            }
            else
            {
                // インデックスが範囲外の場合はエラーハンドリングを行う
                return Vector3.zero; // または適切なデフォルト値を返す
            }
        }
        else
        {
            // 変数が見つからない場合のエラーハンドリング
            return Vector3.zero; // または適切なデフォルト値を返す
        }
    }

    public int GetArrayLength(int arrayIndex)
    {
        // 変数名を構築
        string variableName = "v3PlayerPos" + arrayIndex;

        // フィールドを取得して配列の長さを取得
        FieldInfo field = GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            Vector3[] array = (Vector3[])field.GetValue(this);
            return array.Length;
        }
        else
        {
            // 変数が見つからない場合のエラーハンドリング
            return 0; // または適切なデフォルト値を返す
        }
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
        
        //iPowerGageYPos = 0;
        CreateBox();
        CPUmodeInit();
    }

    protected virtual void CPUmodeInit(){
    }

    public void AddDummyPlayer(int playercnt, Vector3 v3){
        string canvasName = GetCanvasName() + playercnt;
        GameObject gCanvas = LoadResource(canvasName);
        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        gCanvas.transform.position = v3PwrGage;

        GameObject gPlayer = LoadResource(GetPlayerName() + playercnt);
        gPlayer.name = "PlayerDummy" + playercnt;
        gPlayer.tag = "Player_DummyMode";
        gPlayer.transform.position = v3;

        // gPlayerにアタッチされているPlayerスクリプトを取得
        Player playerComponent = gPlayer.GetComponent<Player>();
        if (playerComponent != null)
        {
            // Playerスクリプトを削除
            Destroy(playerComponent);
        }
        playerComponent = gPlayer.GetComponent<Player_CpuMode>();
        if (playerComponent != null)
        {
            // Playerスクリプトを削除
            Destroy(playerComponent);
            //CuurentPlayerNum++;
        }

        //CPUモードに切り替え
        Player cPlayer = AddComponent(gPlayer);
        cPlayer.MaterialType = "BomMaterial"+playercnt;
        m_playerCount++;
        
        cPlayer.SetViewID(m_playerCount);
        cPlayer.SetSlider(gCanvas);
        SetupSlider(m_playerCount, gCanvas);
    }

    protected virtual string GetCanvasName(){
        return "CanvasOnline";
    }

    protected virtual string GetPlayerName(){
        return "PlayerOnline";
    }

    protected virtual Player AddComponent(GameObject gPlayer){
        Player cPlayer = gPlayer.AddComponent<Player>();
        return cPlayer;
    }



    protected virtual void SpawnPlayerObjects(int playerCount)
    {
        m_playerCount = playerCount;
        for (int i = 1; i <= playerCount; i++)
        {
            string canvasName = "Canvas" + i;
            GameObject gCanvas = LoadResource(canvasName);
            Vector3 v3PwrGage = new Vector3(0, 0, 0);
            gCanvas.transform.position = v3PwrGage;

            string PlayerName = "Player" + i;
            GameObject gPlayer = LoadResource(PlayerName);
            Player cPlayer;
            if(i == 1){
                cPlayer = gPlayer.GetComponent<Player>();
            }
            else{
                cPlayer = gPlayer.GetComponent<Player_CpuMode>();
            }
            
            SetupPlayer(gPlayer, i, gCanvas);
        }
    }

    protected void SetupPlayer(GameObject gPlayer, int i, GameObject gCanvas)
    {
        gPlayer.transform.position = GetPlayerPosition(GetIndex(),i-1);

        Player cPlayer = gPlayer.GetComponent<Player_CpuMode>(); // デフォルトでは Player_CpuMode を取得する
        if (i == 1)
        {
            cPlayer = gPlayer.GetComponent<Player>();
        }
        cPlayer.SetViewID(i); // カウンタiを使用する
        cPlayer.SetSlider(gCanvas);
        SetupSlider(i, gCanvas);
    }

    private void SetupSlider(int i, GameObject gCanvas)
    {
        Debug.Log("PoewrGage:"+i);
        Slider slider = gCanvas.GetComponentInChildren<Slider>(); // Canvasの子要素からSliderを取得します。
        RectTransform sliderRectTransform = slider.GetComponent<RectTransform>(); // SliderのRectTransformを取得します。
        Vector3 newPosition = sliderRectTransform.position; // 現在の位置を取得します。
        newPosition.y = 260 - (i - 1) * 20;
        sliderRectTransform.position = newPosition; // 新しい座標を設定します。        
    }


    protected virtual int GetIndex(){
        return 1;
    }

    protected GameObject LoadResource(string loadname){
        // Resourcesフォルダ内のPlayer1プレハブを読み込む
        GameObject playerPrefab = Resources.Load<GameObject>(loadname);
        return Instantiate(playerPrefab);

    }


    // Update is called once per frame
    void Update()
    {
        GameTransision();        
    }
/*
    // playerCountを引数に取り、v3PlayerPos[playerCount-1]を返す関数
    public virtual Vector3 GetPlayerPosition(int playerCount, int index)
    {
        if (playerCount >= 1 && playerCount <= GetArrayLength(index).Length)
        {
            return GetPlayerPosition(index,playerCount - 1);
        }
        else
        {
            Debug.LogError("Invalid playerCount: " + playerCount);
            return Vector3.zero; // エラー時は適当な値を返すか、エラーハンドリングを行う
        }
    }
*/

    public void AddBrokenBlock(){
        int y = 1;
        for (int x = 0; x < GameManager.xmax; x++) {
            for (int z = 0; z < GameManager.zmax; z++) {
                Vector3 v3 = new Vector3(x,y,z);
                if(false == CheckPosition(v3)){
                    int iRand = Random.Range(0, 10);
                    if(0 == iRand){
                        InsBrokenBlock_RPC(x, y, z);
                    }
                }
            }
        }
    }


    public void CreateBrokenBlock(){
        ClearBrokenList_RPC();
        int y = 1;
        for (int x = 0; x < GameManager.xmax; x++) {
            for (int z = 0; z < GameManager.zmax; z++) {
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
        if ((x == 1 || x == 2 || x == GameManager.xmax-3 || x == GameManager.xmax-2) && (z == GameManager.zmax-3 || z == GameManager.zmax-2 || z == 1 || z == 2))
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

    protected virtual void InsObjMove_RPC(int x, int y, int z, Direction randomDirection){
        photonView.RPC(nameof(InsObjMove), RpcTarget.All, x, y, z, randomDirection);
    }

    [PunRPC]
    public void InsObjMove(int x, int y, int z, Direction randomDirection)
    {
        ObjMovePrefab = Resources.Load<GameObject>("ObjMove");
        GameObject g = Instantiate(ObjMovePrefab);
        g.transform.position = new Vector3(x, y, z);

        // 向きを変更し、移動方向を設定する
        SetDirection(g, randomDirection);

        ObjMoveList.Add(g);
    }

    // 方向に応じて回転と移動方向を設定する関数
    private void SetDirection(GameObject obj, Direction direction)
    {
        Quaternion rotation = Quaternion.identity;
        Vector3 moveDirection = Vector3.zero;

        switch (direction)
        {
            case Direction.Up:
                rotation = Quaternion.Euler(0, -90, 0);
                moveDirection = Vector3.forward; // Z方向に移動する
                break;
            case Direction.Down:
                rotation = Quaternion.Euler(0, 90, 0);
                moveDirection = Vector3.back; // Z方向に逆向きに移動する
                break;
            case Direction.Left:
                rotation = Quaternion.Euler(0, 0, 180); // 左向きの回転
                moveDirection = Vector3.left; // X方向に進む
                break;
            case Direction.Right:
                rotation = Quaternion.Euler(0, 0, 0); // 右向きの回転
                moveDirection = Vector3.right; // X方向に進む
                break;
            default:
                break;
        }

        obj.transform.rotation = rotation;
        obj.GetComponent<ObjMove>().SetMoveDirection(moveDirection);
    }


    private void CreateExplode(Vector3 v3){
        GameObject g = Instantiate(GroundExplosionPrefab);            
        string sMaterialType = GetBomMaterial(v3, GetIndex());
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

    protected virtual string GetBomMaterial(Vector3 target, int index)
    {
        target.y += 1;

        // v3PlayerPosの各要素と比較
        for (int i = 0; i < GetArrayLength(index); i++)
        {
            if (GetPlayerPosition(index,i) == target)
            {
                // 一致する要素が見つかった場合、該当する文字列を返す
                return "BomMaterial" + (i + 1);
            }
        }

        return "InvalidMaterial";
    }


/*
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
*/
    private void CreateBox(){
        for (int x = 0; x < GameManager.xmax; x++) {
            for (int z = 0; z < GameManager.zmax; z++) {
                //int iRand = Random.Range(0, 10);
                int iRand = 0;
                int y1 = 0;
                int y2 = 1;

                //矢印ブロックは生成しないようにしている。オンラインの場合、このタイミングで生成しようとすると、Pothon実行前と思われ、エラーになってしまうため。
                //オンラインに対応するのであれば、破壊ブロックと同じタイミングでPothonから生成するようにする。
                if(1 == iRand){
                    Direction randomDirection = GetRandomDirection();
                    InsObjMove_RPC(x, y1, z, randomDirection);
                }
                else{
                    //Instantiateはprehabの抽象データBoxを複製して実体化する
                    GameObject g1 = Instantiate(GroundPrefab);
                    g1.transform.position = new Vector3(x, y1, z);
                    GroundList.Add(g1);
                }

                
                if(x == 0 || z == 0 || x == GameManager.xmax-1 || z == GameManager.zmax-1){
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

    // 方向を表す列挙型
    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    // ランダムな方向を取得する関数
    public Direction GetRandomDirection()
    {
        return (Direction)Random.Range(0, System.Enum.GetValues(typeof(Direction)).Length);
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

public bool CheckPosition(Vector3 targetPosition)
{
    // シーン内の全てのGameObjectを取得
    GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

    // 全てのGameObjectに対してループ処理を行う
    foreach (GameObject obj in allGameObjects)
    {
        // GameObjectのTransformコンポーネントを取得
        Transform objTransform = obj.transform;

        // アクティブかつ座標が一致するかをチェックする
        if (obj.activeInHierarchy && objTransform.position == targetPosition)
        {
            // 一致する場合はtrueを返す
            return true;
        }
    }

    // 一致するGameObjectが見つからない場合はfalseを返す
    return false;
}



    public void DeletePositionAndName(Vector3 targetPosition, string targetName)
    {
        // シーン内の全てのGameObjectを取得
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
        GameObject g = null;

        // 全てのGameObjectに対してループ処理を行う
        foreach (GameObject obj in allGameObjects)
        {
            // GameObjectのTransformコンポーネントを取得
            Transform objTransform = obj.transform;

            // アクティブかつ座標と名称が一致するかをチェックする
            if (obj.activeInHierarchy && objTransform.position == targetPosition && obj.name == targetName)
            {
                g = obj;
                break;
            }
        }
        if(null != g){
            Destroy(g);
        }
        

        // 一致するGameObjectが見つからない場合はfalseを返す
        return;
 
 
    }

    
    public void SetName(string namepara){
        PlayerName = namepara;
    }

    public string GetName(){
        return PlayerName;
    }

    public void SetPlayerNum(int num){
        CuurentPlayerNum = num;
    }
    public int GetPlayerNum(){
        return CuurentPlayerNum;
    }

    public void DeadPlayer(string tag){
        /*
        if(tag == "Player"){
            GameOver();
            return;
        }

        if(tag != "Player" && tag != "Player_Online"){
            CuurentPlayerNum--;
        }
        
        Debug.Log(CuurentPlayerNum);
        if(0 >= CuurentPlayerNum){
            Debug.Log("Win");
            GameWin();
        }
        */
        
    }

    public void GameWin(){
        int iStage = GameManager.NextStage();
        if(iStage <= 5){
            Invoke("SwitchGameScene", 5f);
        }
        else{
            GameOver();
        }
        
    }

    public void GameOver(){
        Invoke("SwitchGameOver", 5f);
    }

    void SwitchGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    void SwitchGameOver()
    {
        SceneManager.LoadScene("GameTitle");
    }

    protected virtual void GameTransision()
    {
    }

    int CountObjectsWithName(string name)
    {
        int count = 0;
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in objects)
        {
            if (obj.name == name)
            {
                count++;
            }
        }
        return count;
    }

    [PunRPC]
    public void Rainbow(string sMaterialType){

        MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        Material cMaterial = cMaterialMng.GetMaterialOfType(sMaterialType);

        List<GameObject> objectsToRemove = new List<GameObject>();
        List<GameObject> newExplosionList = new List<GameObject>(); // 新しいリストを作成

        foreach (GameObject obj in ExplosionList)
        {
            if (obj != null)
            {
                Vector3 v3 = obj.transform.position;

                GameObject gObj = Instantiate(GroundExplosionPrefab);
                gObj.GetComponent<Renderer>().material = cMaterial;
                gObj.transform.position = v3;

                // 新しいリストに追加
                newExplosionList.Add(gObj);

                objectsToRemove.Add(obj);
            }
        }

        // 元のリストから削除
        foreach (GameObject objToRemove in objectsToRemove)
        {
            ExplosionList.Remove(objToRemove);
            Destroy(objToRemove);
        }

        
        foreach (GameObject objToAdd in newExplosionList)
        {
            objToAdd.GetComponent<Explosion>().FieldValid();
            AddExplosion(objToAdd);
        }
        
    }



    protected virtual void Rainbow_RPC(string sMaterialType){
        photonView.RPC(nameof(Rainbow), RpcTarget.All, sMaterialType);
    }


}
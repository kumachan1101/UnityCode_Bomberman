using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Library_Base : MonoBehaviour{

    private static Library_Base instance; // GameManagerのシングルトンインスタンス
    public static Library_Base Instance
    {
        get
        {
            // インスタンスがまだ作成されていない場合は、新しいGameManagerを作成します。
            if (instance == null)
            {
                GameObject gameObject = new GameObject("Library");
                instance = gameObject.AddComponent<Library_Base>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 既にGameManagerのインスタンスが存在する場合、新しいインスタンスを破棄します。
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
   public static int CountObjectsWithName(string name)
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

    public static int GetPlayerCnt()
    {
        // シーン内のすべてのGameObjectを取得
        GameObject[] allPlayers = GameObject.FindObjectsOfType<GameObject>();

        // Playerで始まるGameObjectのカウント
        int playerCount = 0;
        foreach (GameObject player in allPlayers)
        {
            if (player.name.StartsWith("Player"))
            {
                playerCount++;
            }
        }

        return playerCount;
    }

    // 指定した座標と名称が一致し、かつアクティブであるオブジェクトが存在するかをチェックする関数
    public static bool CheckPositionAndName(Vector3 targetPosition, string targetName)
    {
        // シーン内の全てのGameObjectを取得
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        // 全てのGameObjectに対してループ処理を行う
        foreach (GameObject obj in allGameObjects)
        {
            // GameObjectのTransformコンポーネントを取得
            Transform objTransform = obj.transform;

            // アクティブかつ座標と名称が一致するかをチェックする
            if (obj.activeInHierarchy && objTransform.position == targetPosition && obj.name.Contains(targetName))
            {
                // 一致する場合はtrueを返す
                return true;
            }
        }

        // 一致するGameObjectが見つからない場合はfalseを返す
        return false;
    }

    static public GameObject GetPositionAndNameObj(Vector3 targetPosition, string targetName)
    {
        // シーン内の全てのGameObjectを取得
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        // 全てのGameObjectに対してループ処理を行う
        foreach (GameObject obj in allGameObjects)
        {
            // GameObjectのTransformコンポーネントを取得
            Transform objTransform = obj.transform;

            // アクティブかつ座標と名称が一致するかをチェックする
            if (obj.activeInHierarchy && objTransform.position == targetPosition && obj.name.Contains(targetName))
            {
                // 一致する場合はtrueを返す
                return obj;
            }
        }

        // 一致するGameObjectが見つからない場合はfalseを返す
        return null;
    }


    public static bool CheckPosition(Vector3 targetPosition)
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



    static public GameObject IsPositionAndName(Vector3 targetPosition, string targetName)
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
            if (obj.activeInHierarchy && objTransform.position == targetPosition && obj.name.Contains(targetName))
            {
                g = obj;
                break;
            }
        }
        return g;
    }
    static public Vector3 GetPos(Vector3 position)
    {
        float x = Mathf.Round(position.x);
        float y = 1;
        float z = Mathf.Round(position.z);
        return new Vector3(x, y, z);
    }

	public int GetPhotonUniqueID(){
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        HashSet<int> usedIDs = new HashSet<int>();

        foreach (var view in photonViews)
        {
            usedIDs.Add(view.ViewID);
        }

        // 新しいIDを生成するか確認するロジック
        int newID = GenerateUniqueID(usedIDs);

        if (usedIDs.Contains(newID))
        {
			Debug.LogError("Duplicate PhotonView ID detected. Instantiation aborted.");
			return -1;
		}
		return newID;
	}

    private int GenerateUniqueID(HashSet<int> usedIDs)
    {
        int newID = Random.Range(1, 10000); // 適切な範囲で新しいIDを生成
        while (usedIDs.Contains(newID))
        {
            newID = Random.Range(1, 10000);
        }
        return newID;
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

    public static PlayerBom GetPlayerBomFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        PlayerBom cPlayerBom = gPlayer.GetComponent<PlayerBom>();
        return cPlayerBom;
    }
/*
    public static BomConfigurationBase GetBomConfigurationFromObject(string objname, BomConfigurationType configType)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        PlayerBom cPlayerBom = null; // PlayerBomコンポーネントの参照を初期化
        BomConfigurationBase cBomConfiguration = null;
        Player_Base cPlayer = GetcPlayerFromObject(objname); // プレイヤー取得

        if (cPlayer != null)
        {
            cPlayerBom = cPlayer.GetPlayerBom(); // プレイヤーのボムコンフィグを取得
        }

        if (cPlayerBom != null)
        {
            // BomConfigurationType に基づいて派生クラスを取得
            cBomConfiguration = cPlayerBom.GetBomConfiguration();
        }

        return cBomConfiguration;
    }


    public static BomStatus GetBomStatusFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        PlayerBom cPlayerBom = null; // PlayerBomコンポーネントの参照を初期化
		BomStatus cBomStatus = null;
        Player_Base cPlayer = GetcPlayerFromObject(objname);
        if (cPlayer != null)
        {
            cPlayerBom = cPlayer.GetPlayerBom();
        }
		if(cPlayerBom != null){
			cBomStatus = cPlayerBom.GetBomStatus();
		}

        return cBomStatus;
    }
    public static BomManagement GetBomManagementFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        PlayerBom cPlayerBom = null; // PlayerBomコンポーネントの参照を初期化
		BomManagement cBomManagement = null;
        Player_Base cPlayer = GetcPlayerFromObject(objname);
        if (cPlayer != null)
        {
            cPlayerBom = cPlayer.GetPlayerBom();
        }
		if(cPlayerBom != null){
			cBomManagement = cPlayerBom.GetBomManagement();
		}

        return cBomManagement;
    }
*/

    public static PlayerAction GetcPlayerActionFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        PlayerAction cPlayerAction = gPlayer.GetComponent<PlayerAction>();
        return cPlayerAction;
    }

    public static Player_Base GetcPlayerFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        Player_Base cPlayer = null;
        if (gPlayer == null)
        {
            Debug.LogError("Object with the name not found.");
            return cPlayer;
        }
        cPlayer = gPlayer.GetComponent<Player_Base>();
		return cPlayer;
    }

    public static int ExtractNumberFromString(string input)
    {
        // 正規表現パターンを定義
        string pattern = @"\d+"; // 数字を表すパターン
        
        // 正規表現を使ってパターンにマッチする部分を抜き出す
        Match match = Regex.Match(input, pattern);
        
        // マッチした部分をint型に変換して返す
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        else
        {
            // マッチしなかった場合は0を返すが、適切なエラーハンドリングが必要であれば適宜修正してください
            return 0;
        }
    }

    public static GameObject FindPlayerObject(string name)
    {
        return GameObject.Find(name);
    }

    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    // ランダムな方向を取得する関数
    public static Direction GetRandomDirection()
    {
        return (Direction)Random.Range(0, System.Enum.GetValues(typeof(Direction)).Length);
    }

    public static void SetDirection(GameObject obj, Direction direction)
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


    /// <summary>
    /// 指定した座標に既にGameObjectが存在するかを判定します。
    /// </summary>
    /// <param name="position">確認する座標</param>
    /// <returns>その座標にGameObjectが存在する場合はtrue、存在しない場合はfalseを返します。</returns>
    public static bool IsGameObjectAtPosition(Vector3 position)
    {
        // 指定した座標に存在するすべてのコライダーを取得
        Collider[] colliders = Physics.OverlapSphere(position, 0.25f);

        // コライダーが1つ以上存在すればtrueを返す
        if (colliders.Length > 0)
        {
            return true;
        }

        // 存在しなければfalseを返す
        return false;
    }

	public static bool IsPositionOutOfBounds(Vector3 v3)
	{
		if (GameManager.xmax - 2 <= v3.x || GameManager.zmax - 2 <= v3.z || 2 > v3.x || 2 > v3.z)
		{
			return true;
		}
		return false;
	}

    public static GameObject FindGameObjectByInstanceID(int instanceID)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.GetInstanceID() == instanceID)
            {
                return obj;
            }
        }
        return null; // 見つからない場合
    }

}
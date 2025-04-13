using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectGenerator
{
    // キャンバスを生成
    public GameObject InstantiateCanvas(string canvasName)
    {
        GameObject gCanvas = LoadResource(canvasName);
        gCanvas.transform.position = new Vector3(0, 0, 0);
        return gCanvas;
    }

    // プレイヤーを生成
    public GameObject InstantiatePlayer(string playerName, Vector3 position)
    {
        GameObject gPlayer = LoadResource(playerName);
        gPlayer.transform.position = position;
        return gPlayer;
    }

    // Resourcesフォルダからリソースを読み込み
    private GameObject LoadResource(string loadName)
    {
        GameObject prefab = Resources.Load<GameObject>(loadName);
        return prefab != null ? Object.Instantiate(prefab) : null;
    }
}

public interface IPlayerFactory
{
    GameObject CreatePlayer(string playerName, Vector3 position);
}

// --- Base Factory (共通処理をまとめる) ---
public abstract class BasePlayerFactory : IPlayerFactory
{
    public GameObject CreatePlayer(string playerName, Vector3 position)
    {
        PlayerObjectGenerator generator = new PlayerObjectGenerator();
        GameObject player = generator.InstantiatePlayer("Player", position);
        player.name = playerName;
        if (player != null)
        {
            AddPlayerComponent(player);
        }
        return player;
    }

    // 各派生クラスで追加するコンポーネントを定義
    protected abstract void AddPlayerComponent(GameObject player);
}

// --- 通常プレイヤー用のファクトリ ---
public class StandardPlayerFactory : BasePlayerFactory
{
    protected override void AddPlayerComponent(GameObject player)
    {
        player.AddComponent<Player>(); // 通常プレイヤー用のコンポーネント
    }
}

// --- CPU プレイヤー用のファクトリ ---
public class CpuPlayerFactory : BasePlayerFactory
{
    protected override void AddPlayerComponent(GameObject player)
    {
        player.AddComponent<Player_CpuMode>(); // CPUモードプレイヤー用のコンポーネント
    }
}

// --- プレイヤーの種類に応じて適切なファクトリを取得 ---
public class PlayerFactorySelector
{
    public static IPlayerFactory GetFactory(int playerNo)
    {
        return playerNo == 1 ? new StandardPlayerFactory() : new CpuPlayerFactory();
    }
}


public class PlayerPositionManager_CpuMode : PlayerPositionManager {

    public override void SetPlayerPositions()
    {
        int xmax = GameManager.xmax;
        int zmax = GameManager.zmax;

        // プレイヤー数を4以上のランダムな値に設定（例: 4〜10人）
        int playerCount = Random.Range(4, 4);

        // プレイヤー位置のリストを初期化
        playerPositions = new Vector3[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            Vector3 randomPosition;

            // 他のプレイヤーと被らないようにランダムな位置を選定
            do
            {
                randomPosition = new Vector3(
                    Random.Range(3, xmax - 3), // フィールドの端を避ける
                    0.5f,
                    Random.Range(3, zmax - 3)
                );
            } while (IsPositionOccupied(randomPosition));

            playerPositions[i] = randomPosition;
        }
    }


	// 他のプレイヤー位置と重複しないかチェックする関数
	private bool IsPositionOccupied(Vector3 position)
	{
	    foreach (Vector3 existingPosition in playerPositions)
	    {
	        if (existingPosition == position)
	        {
	            return true;
	        }
	    }
	    return false;
	}
}

public class PlayerPowerManager_CpuMode: PlayerPowerManager
{
    public override int GetPower(){
        return 5;
    }

}

public class PlayerSpawnManager_CpuMode : PlayerSpawnManager {

    protected override void InitEvent (){
        eventDispatcher = GameObject.Find("EventDispatcher").GetComponent<EventDispatcher>();
        eventDispatcher.RegisterListener<CompleteBlockCreateEvent>(OnCompleteBlockCreateEvent);
    }

    void OnDestroy()
    {
        eventDispatcher.UnregisterListener<CompleteBlockCreateEvent>(OnCompleteBlockCreateEvent);
    }


    //private void OnCompleteBlockCreateEvent(CompleteBlockCreateEvent eEvent){
    private void OnCompleteBlockCreateEvent(IEvent eEvent){
        //Debug.Log("OnCompleteBlockCreateLog");
        RequestPlayerSpawn();
    } 

    public virtual void RequestPlayerSpawn() {
        int iPlayerCnt = InitializePlayerPositionsAndGetCount();
        UpdatePlayerCount(iPlayerCnt);
        SetInitialPlayerName();
        SpawnPlayers(iPlayerCnt);
    }

    private int InitializePlayerPositionsAndGetCount() {
        cPlayerPositionManager.SetPlayerPositions();
        return cPlayerPositionManager.GetPlayerCount();
    }

    private void UpdatePlayerCount(int iPlayerCnt) {
        cPlayerCountManager.SetPlayerCount(iPlayerCnt);
    }

    private void SetInitialPlayerName() {
        cPlayerNameManager.SetPlayerName("Player1");
    }

    private void SpawnPlayers(int iPlayerCnt) {
        for (int i = 1; i <= iPlayerCnt; i++) {
            int playerNo = DeterminePlayerNumber(i);
            SpawnPlayer(playerNo);
        }
    }

    protected virtual int DeterminePlayerNumber(int index) {
        return (index == 1) ? 1 : Random.Range(2, 4);
    }


	protected override bool PreSpawnDummyPlayer(){
		cPlayerCountManager.AddPlayerCount();
		return true;
	}
    protected override void PlayerAddConponent (){
        cPlayerPositionManager = gameObject.AddComponent<PlayerPositionManager_CpuMode>();
        gameObject.AddComponent<PlayerPowerManager_CpuMode>();
            
    }

    public override void SpawnDummyPlayer(int playerNo, Vector3 position)
    {
        var cpuFactory = new CpuPlayerFactory(); // BaseFactory を使う
        CreateAndSetupPlayer(playerNo, position, cpuFactory);
    }

    public override void SpawnPlayer(int playerNo)
    {
        Vector3 position = cPlayerPositionManager.GetPlayerPosition(playerNo - 1); // プレイヤー位置を取得
        var factory = PlayerFactorySelector.GetFactory(playerNo);
        CreateAndSetupPlayer(playerNo, position, factory);
    }
    // プレイヤーとキャンバスを設定するメソッド
    private void CreateAndSetupPlayer(int playerNo, Vector3 position, IPlayerFactory playerFactory)
    {
        string canvasName = "";
        string playerName = "";
        cPlayerNameManager.GetPlayerNames(playerNo, ref canvasName, ref playerName);

        PlayerObjectGenerator generator = new PlayerObjectGenerator();
        GameObject canvas = generator.InstantiateCanvas("CanvasPowerGage");
        SetupCanvas(canvas, playerNo); // キャンバスの設定

        GameObject player = playerFactory.CreatePlayer(playerName, position);
        SetupPlayer(player, canvas.GetInstanceID()); // プレイヤーの設定
    }

    private void SetupCanvas(GameObject canvas, int playerNo)
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas is null. Cannot set up.");
            return;
        }

        var powerGageSlider = canvas.GetComponent<PowerGage_Slider>();
        if (powerGageSlider != null)
        {
            powerGageSlider.SetPlayerNo(playerNo);
        }
        else
        {
            Debug.LogWarning("PowerGage_Slider component is missing on the canvas.");
        }
    }

    private void SetupPlayer(GameObject player, int canvasID)
    {
        if (player == null)
        {
            Debug.LogError("Player is null. Cannot set up.");
            return;
        }

        //player.name = playerName;

        var powerGageIF = player.AddComponent<PowerGageIF_CpuMode>();
        if (powerGageIF != null)
        {
            powerGageIF.SetCanvasInsID(canvasID);
        }
        else
        {
            Debug.LogWarning("Failed to add PowerGageIF_CpuMode to the player.");
        }

        //RearrangeCanvases();

        var playerBase = player.GetComponent<Player_Base>();
        if (playerBase != null)
        {
            playerBase.AddPlayerComponent();
        }
        else
        {
            Debug.LogWarning("Player_Base component is missing on the player.");
        }
    }



}

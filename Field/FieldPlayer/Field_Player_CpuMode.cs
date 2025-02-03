using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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


// プレイヤー生成用のインターフェース
public interface IPlayerFactory
{
    GameObject CreatePlayer(string playerName, Vector3 position);
}

public class DummyPlayerFactory : IPlayerFactory
{
    public GameObject CreatePlayer(string playerName, Vector3 position)
    {
        PlayerObjectGenerator generator = new PlayerObjectGenerator();
        GameObject player = generator.InstantiatePlayer("Player", position);
        player.name = playerName;
        if (player != null)
        {
            player.AddComponent<Player_CpuMode>(); // ダミープレイヤー用のコンポーネントを追加
        }
        return player;
    }
}

public class StandardPlayerFactory : IPlayerFactory
{

    public GameObject CreatePlayer(string playerName, Vector3 position)
    {
        PlayerObjectGenerator generator = new PlayerObjectGenerator();
        GameObject player = generator.InstantiatePlayer("Player", position);
        player.name = playerName;
        if (player != null)
        {
            char lastChar = playerName[playerName.Length - 1];
            int playerNo = int.Parse(lastChar.ToString());
            if (playerNo == 1)
            {
                player.AddComponent<Player>(); // 通常プレイヤー用のコンポーネントを追加
            }
            else
            {
                player.AddComponent<Player_CpuMode>(); // CPUモードプレイヤー用のコンポーネントを追加
            }
        }
        return player;
    }
}


public class Field_Player_CpuMode : Field_Player_Base {
    protected virtual void GetCPUPlayerInfo(ref string canvasName, ref string playerName){}
    public virtual void SetPower(Slider cSlider){}
    public virtual int GetPower(){
        return 10;
    }

	protected override bool PreAddDummyPlayer(){
		cPlayerCountManager.AddPlayerCount();
		return true;
	}

    public virtual string GetBomMaterial(Vector3 target, int index)
    {
		return "";
    }

    public override void AddDummyPlayer(int playerNo, Vector3 position)
    {
        var dummyFactory = new DummyPlayerFactory();
        CreateAndSetupPlayer(playerNo, position, dummyFactory);
    }

    public override void SpawnPlayerObjects(int playerNo)
    {
        Vector3 position = GetPlayerPosition(GetIndex(), playerNo - 1); // プレイヤー位置を取得
        var standardFactory = new StandardPlayerFactory();
        CreateAndSetupPlayer(playerNo, position, standardFactory);
    }
    // プレイヤーとキャンバスを設定するメソッド
    private void CreateAndSetupPlayer(int playerNo, Vector3 position, IPlayerFactory playerFactory)
    {
        string canvasName = "";
        string playerName = "";
        GetPlayerNames(playerNo, ref canvasName, ref playerName);

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

        RearrangeCanvases();

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

    /// <summary>
    /// CanvasPowerGageを上詰めで再配置する (SetPlayerCntを利用)
    /// </summary>
    private void RearrangeCanvases()
    {
        // "CanvasPowerGage" 名称を持つオブジェクトをすべて取得
        List<GameObject> canvasPowerGages = Library_Base.FindGameObjectsByPartialName("CanvasPowerGage(Clone)");
        if (canvasPowerGages.Count == 0)
        {
            Debug.LogWarning("CanvasPowerGage が見つかりませんでした。");
            return;
        }

        // ソート: Canvas をプレイヤー番号順に並べ替え
        canvasPowerGages.Sort((a, b) =>
        {
            int noA = a.GetComponent<PowerGage_Slider>().GetPlayerNo();
            int noB = b.GetComponent<PowerGage_Slider>().GetPlayerNo();
            return noA.CompareTo(noB);
        });

        // 再配置処理
        int newPlayerCnt = 1; // 新しいプレイヤーのカウント
        foreach (GameObject canvas in canvasPowerGages)
        {
            PowerGage_Slider powerGageSlider = canvas.GetComponent<PowerGage_Slider>();
            if (powerGageSlider != null)
            {
                //Debug.Log(newPlayerCnt);
                powerGageSlider.SetPlayerCnt(newPlayerCnt); // 新しい位置を設定
                newPlayerCnt++; // 次の位置のためにカウントを増やす
            }
        }
    }

}
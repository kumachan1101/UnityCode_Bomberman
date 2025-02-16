using UnityEngine;
using Photon.Pun;
public class Field_CpuMode :Field_Event{

    private GameManager cGameManager;

    // プレイヤーの追加・削除イベントリスナーを登録
    protected override void RegisterListeners()
    {
        Player_Base.onPlayerAdded.AddListener(Field_CpuMode_OnAdded);
        Player_Base.onPlayerRemoved.AddListener(Field_CpuMode_OnRemoved);
    }
    protected override void UnregisterListeners()
    {
        Player_Base.onPlayerAdded.RemoveListener(Field_CpuMode_OnAdded);
        Player_Base.onPlayerRemoved.RemoveListener(Field_CpuMode_OnRemoved);
    }


	protected override void Init()
	{
		cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}


    // プレイヤー追加時の処理
    private void Field_CpuMode_OnAdded(object obj)
    {
        //Debug.Log($"{player.name} がゲームに追加されました");
        GameTransision();  // ゲーム進行チェックを呼び出す
    }

    // プレイヤー削除時の処理
    private void Field_CpuMode_OnRemoved(object obj)
    {
        //Debug.Log($"{player.name} がゲームから削除されました");
        GameTransision();  // ゲーム進行チェックを呼び出す
    }

    protected void GameTransision()
    {
        bool hasPlayer1 = false;
        bool hasPlayerDummy1 = false;

        // "Player1(Clone)" または "PlayerDummy1" の存在を確認します
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Player1")
            {
                hasPlayer1 = true;
            }
            else if (obj.name == "PlayerDummy1")
            {
                hasPlayerDummy1 = true;
            }

            // 必要な条件が満たされた場合、ループを終了します
            if (hasPlayer1 || hasPlayerDummy1)
            {
                break;
            }
        }
        // ゲームクリアに必要な条件が満たされているかどうかを確認します
        if (hasPlayer1 || hasPlayerDummy1)
        {
            // "Player2(Clone)", "Player3(Clone)", "Player4(Clone)", "PlayerDummy2", "PlayerDummy3", "PlayerDummy4" が存在しないかどうかを確認します
            if (GameObject.Find("Player2") == null &&
                GameObject.Find("Player3") == null &&
                GameObject.Find("Player4") == null &&
                GameObject.Find("PlayerDummy2") == null &&
                GameObject.Find("PlayerDummy3") == null &&
                GameObject.Find("PlayerDummy4") == null)
            {
                cGameManager.GameWin();
            }
            else
            {
                //Debug.Log("ゲーム続行");
            }
        }
        else
        {
            cGameManager.GameOver();
        }
    }

}
using UnityEngine;
using Photon.Pun;

public class PlayerAddedRemoved : Field_Event
{
    private GameManager cGameManager;
    private PlayerAddedRemovedHandler playerHandler;

    protected override void Init()
    {
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerHandler = new PlayerAddedRemovedHandler(cGameManager);
    }

    // プレイヤーの追加・削除イベントリスナーを登録
    protected override void RegisterListeners()
    {
        Player_Base.onPlayerAdded.AddListener(playerHandler.OnAdded);
        Player_Base.onPlayerRemoved.AddListener(playerHandler.OnRemoved);
    }

    protected override void UnregisterListeners()
    {
        Player_Base.onPlayerAdded.RemoveListener(playerHandler.OnAdded);
        Player_Base.onPlayerRemoved.RemoveListener(playerHandler.OnRemoved);
    }
}

public class PlayerAddedRemovedHandler
{
    private GameManager cGameManager;

    public PlayerAddedRemovedHandler(GameManager gameManager)
    {
        cGameManager = gameManager;
    }

    // プレイヤー追加時の処理
    public void OnAdded(object obj)
    {
        GameTransision();  // ゲーム進行チェックを呼び出す
    }

    // プレイヤー削除時の処理
    public void OnRemoved(object obj)
    {
        GameTransision();  // ゲーム進行チェックを呼び出す
    }

    // ゲーム進行のチェック
    private void GameTransision()
    {
        bool hasPlayer1 = false;
        bool hasPlayerDummy1 = false;

        // "Player1(Clone)" または "PlayerDummy1" の存在を確認
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

            // 必要な条件が満たされた場合、ループを終了
            if (hasPlayer1 || hasPlayerDummy1)
            {
                break;
            }
        }

        // ゲームクリア条件チェック
        if (hasPlayer1 || hasPlayerDummy1)
        {
            if (GameObject.Find("Player2") == null &&
                GameObject.Find("Player3") == null &&
                GameObject.Find("Player4") == null &&
                GameObject.Find("PlayerDummy2") == null &&
                GameObject.Find("PlayerDummy3") == null &&
                GameObject.Find("PlayerDummy4") == null)
            {
                cGameManager.GameWin();
            }
        }
        else
        {
            cGameManager.GameOver();
        }
    }
}

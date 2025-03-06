using UnityEngine;

public class TowerAddedRemoved_GameControl : Field_Event
{
    private GameManager cGameManager;
    private TowerAddedRemovedHandler towerHandler;

    protected override void Init()
    {
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        towerHandler = new TowerAddedRemovedHandler(cGameManager);
    }

    // タワーの追加・削除イベントリスナーを登録
    protected override void RegisterListeners()
    {
        UnregisterListeners();
        Tower.onAdded.AddListener(towerHandler.OnAdded);
        Tower.onRemoved.AddListener(towerHandler.OnRemoved);
    }

    // タワーのリスナー解除
    protected override void UnregisterListeners()
    {
        Tower.onAdded.RemoveListener(towerHandler.OnAdded);
        Tower.onRemoved.RemoveListener(towerHandler.OnRemoved);
    }
}

public class TowerAddedRemovedHandler
{
    private int removedTowerCount = 0;  // 削除されたタワーのカウント
    private GameManager cGameManager;

    public TowerAddedRemovedHandler(GameManager gameManager)
    {
        cGameManager = gameManager;
    }

    // タワー追加時の処理（今は空ですが、必要に応じて実装）
    public void OnAdded(object obj)
    {
        // 追加処理があればここに書く
    }

    // タワー削除時の処理
    public void OnRemoved(object obj)
    {
        if (obj is Tower tower)
        {
            if (tower.name == "Tower1")
            {
                cGameManager.GameOver();  // タワー1が削除された場合はゲームオーバー
            }
            else
            {
                if (cGameManager.IsGameOver())
                {
                    return;
                }
                removedTowerCount++;
                if (removedTowerCount >= 3)
                {
                    cGameManager.GameTowerWin();  // 削除されたタワーが3個以上の場合は勝利
                }
            }
        }
    }
}

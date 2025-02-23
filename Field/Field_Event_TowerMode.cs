using UnityEngine;
public class Field_Event_TowerMode :Field_Event{

    private GameManager cGameManager;
    private int removedTowerCount = 0;  // 削除されたタワーのカウント


    protected override void RegisterListeners()
    {
        UnregisterListeners();
        Tower.onAdded.AddListener(Field_TowerMode_OnAdded);
        Tower.onRemoved.AddListener(Field_TowerMode_OnRemoved);
    }
    protected override void UnregisterListeners()
    {
        Tower.onRemoved.RemoveListener(Field_TowerMode_OnAdded);
        Tower.onRemoved.RemoveListener(Field_TowerMode_OnRemoved);
    }

    protected override void Init()
    {
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Field_TowerMode_OnAdded(object obj)
    {
    }

    private void Field_TowerMode_OnRemoved(object obj)
    {
        if (obj is Tower tower)
        {
            if (tower.name == "Tower1")
            {
                cGameManager.GameOver();
                //Debug.Log(tower);
                //Debug.Log("GameOver");
            }
            else{
                if(cGameManager.IsGameOver()){
                    return;
                }
                removedTowerCount++;
                if (removedTowerCount >= 3){
                    cGameManager.GameTowerWin();
                    //Debug.Log("GameTowerWin");
                }
            }
        }
    }
}
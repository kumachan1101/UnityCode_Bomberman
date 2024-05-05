using UnityEngine;

public class Field_CpuMode : Field {
    private bool bFlag;

    private GameManager cGameManager;

    void start(){
        bFlag = false;
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    protected override void CPUmodeInit(){
        int playerCount = GetArrayLength(GetIndex());
        GameObject gItemControl = GameObject.Find("ItemControl");
        gItemControl.GetComponent<ItemControl_CpuMode>().SetMaster();

        CreateBrokenBlock();
        SpawnPlayerObjects(playerCount);

        SetName("Player1(Clone)");
        SetPlayerNum(playerCount-1);

    }


    protected override void ClearBrokenList_RPC(){
        ClearBrokenList();
    }


    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        InsBrokenBlock(x, y, z);
    }

    protected override void InsObjMove_RPC(int x, int y, int z, Direction randomDirection){
        InsObjMove(x, y, z, randomDirection);
    }

    protected override void Rainbow_RPC(string sMaterialType){
        Rainbow(sMaterialType);
    }

    protected override string GetCanvasName(){
        return "Canvas";
    }

    protected override string GetPlayerName(){
        return "Player";
    }
    protected override Player AddComponent(GameObject gPlayer){
        Player cPlayer = gPlayer.AddComponent<Player_CpuMode>();
        return cPlayer;
    }

    protected override void GameTransision()
    {
        bool hasPlayer1 = false;
        bool hasPlayerDummy1 = false;

        if(bFlag){
            return; 
        }

        if(null == cGameManager){
            cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        // "Player1(Clone)" または "PlayerDummy1" の存在を確認します
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Player1(Clone)")
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
            if (GameObject.Find("Player2(Clone)") == null &&
                GameObject.Find("Player3(Clone)") == null &&
                GameObject.Find("Player4(Clone)") == null &&
                GameObject.Find("PlayerDummy2") == null &&
                GameObject.Find("PlayerDummy3") == null &&
                GameObject.Find("PlayerDummy4") == null)
            {
                cGameManager.GameWin();
                bFlag = true;
            }
            else
            {
                //Debug.Log("ゲーム続行");
            }
        }
        else
        {
            cGameManager.GameOver();
            bFlag = true;
        }
/*
        // 各オブジェクトの数をログで出力します
        int player1Count = CountObjectsWithName("Player1(Clone)");
        int player2Count = CountObjectsWithName("Player2(Clone)");
        int player3Count = CountObjectsWithName("Player3(Clone)");
        int player4Count = CountObjectsWithName("Player4(Clone)");
        int playerDummy1Count = CountObjectsWithName("PlayerDummy1");
        int playerDummy2Count = CountObjectsWithName("PlayerDummy2");
        int playerDummy3Count = CountObjectsWithName("PlayerDummy3");
        int playerDummy4Count = CountObjectsWithName("PlayerDummy4");

        Debug.Log("Player1(Clone)の数: " + player1Count);
        Debug.Log("Player2(Clone)の数: " + player2Count);
        Debug.Log("Player3(Clone)の数: " + player3Count);
        Debug.Log("Player4(Clone)の数: " + player4Count);
        Debug.Log("PlayerDummy1の数: " + playerDummy1Count);
        Debug.Log("PlayerDummy2の数: " + playerDummy2Count);
        Debug.Log("PlayerDummy3の数: " + playerDummy3Count);
        Debug.Log("PlayerDummy4の数: " + playerDummy4Count);
*/
    }

}
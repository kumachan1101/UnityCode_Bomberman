using UnityEngine;
public class Field_CpuMode : Field_Base {

    private bool bFlag;

    private GameManager cGameManager;

	protected override void Init()
	{
		bFlag = false;
		cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

		GetComponent<Field_Block_Base>().CreateField();

		Field_Player_Base fieldPlayerBase = GetComponent<Field_Player_Base>();
		int playercnt = fieldPlayerBase.GetArrayLength(fieldPlayerBase.GetIndex());
		fieldPlayerBase.SpawnPlayerObjects(playercnt);
		fieldPlayerBase.SetName("Player1(Clone)");
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
    }



}
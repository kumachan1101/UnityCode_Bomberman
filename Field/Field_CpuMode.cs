using UnityEngine;
using Photon.Pun;
public class Field_CpuMode :MonoBehaviour{

    private bool bFlag;

    private GameManager cGameManager;


    void Start()
    {
        Init();
    }

    void Update()
    {
        GameTransision();        
		
    }
	protected void Init()
	{
		bFlag = false;
		cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

    protected void GameTransision()
    {
        bool hasPlayer1 = false;
        bool hasPlayerDummy1 = false;

        if(bFlag){
            return; 
        }

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
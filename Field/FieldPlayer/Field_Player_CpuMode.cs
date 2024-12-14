using UnityEngine;
public class Field_Player_CpuMode : Field_Player_Base {

	public void Start()
	{

		int playercnt = GetArrayLength(GetIndex());
		/*
        for (int i = 1; i <= playercnt; i++)
        {
			SpawnPlayerObjects(i);
			SetPlayerCnt(i); 
		}
		*/
		// SetPlayerCntで毎回人数を設定することでスライダーの座標位置を変えている。。
		SetPlayerCnt(1); 
		SpawnPlayerObjects(1);
        for (int i = 2; i <= playercnt; i++)
        {
			SetPlayerCnt(i); 
			SpawnPlayerObjects(Random.Range(2, 5));
		}
	}

    protected virtual void init(){}

    public override void AddDummyPlayer(int iPlayerNo, Vector3 v3)
    {
        bool bIsMine = PreAddDummyPlayer();
        if (!bIsMine)
        {
            return;
        }

        string canvasName = "";
        string playerName = "";
        GetPlayerNames(iPlayerNo, ref canvasName, ref playerName);

        GameObject gCanvas = InstantiateCanvas("CanvasPowerGage");
        GameObject gPlayer = InstantiatePlayer("Player", v3);
        
        if (gPlayer != null)
        {
            gPlayer.AddComponent<Player_CpuMode>();
        }
        SetupPlayerAndCanvas(iPlayerNo, gCanvas, gPlayer, playerName);
    }


    public override void SpawnPlayerObjects(int iPlayerNo)
    {
        string canvasName = "";
        string playerName = "";
        GetPlayerNames(iPlayerNo, ref canvasName, ref playerName);

        GameObject gCanvas = InstantiateCanvas("CanvasPowerGage");
        GameObject gPlayer = InstantiatePlayer("Player", GetPlayerPosition(GetIndex(), iPlayerNo - 1));

        if (gPlayer != null)
        {
            if (iPlayerNo == 1)
            {
                gPlayer.AddComponent<Player>();
            }
            else
            {
                gPlayer.AddComponent<Player_CpuMode>();
            }
        }
        SetupPlayerAndCanvas(iPlayerNo, gCanvas, gPlayer, playerName);
    }

    private void SetupPlayerAndCanvas(int iPlayerNo, GameObject gCanvas, GameObject gPlayer, string playerName)
    {
        if (gCanvas == null || gPlayer == null)
        {
            return;
        }
        gPlayer.name = playerName;

        gCanvas.GetComponent<PowerGage_Slider>().SetPlayerCnt(GetPlayerCnt());
        gCanvas.GetComponent<PowerGage_Slider>().SetPlayerNo(iPlayerNo);

        PowerGageIF cPowerGageIF = gPlayer.AddComponent<PowerGageIF_CpuMode>();
        cPowerGageIF.SetCanvasInsID(gCanvas.GetInstanceID());

        Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
        cPlayer.AddPlayerComponent();
    }

	protected override bool PreAddDummyPlayer(){
		AddPlayerCnt();
		return true;
	}
    /*
	protected override void PlayerDestroy(GameObject gPlayer){
		PlayerDestroyComponent(gPlayer);
	}
    */

    protected GameObject InstantiateCanvas(string canvasName)
    {
        GameObject gCanvas = LoadResource(canvasName);
        gCanvas.transform.position = new Vector3(0, 0, 0);
        return gCanvas;
    }

    protected GameObject InstantiatePlayer(string playerName, Vector3 position)
    {
		GameObject gPlayer = LoadResource(playerName);
		gPlayer.transform.position = position;
        return gPlayer;
    }

	protected override void DestroySync(GameObject g){
		Destroy(g);
	}

    protected override string GetCanvasName(){
        return "Canvas";
    }

    public override string GetPlayerName(){
        return "Player";
	}

}
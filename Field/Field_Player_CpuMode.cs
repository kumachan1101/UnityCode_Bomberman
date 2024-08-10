using UnityEngine;
public class Field_Player_CpuMode : Field_Player_Base {

	protected override bool PreAddDummyPlayer(){
		AddPlayerCnt();
		return true;
	}

	protected override void PlayerDestroy(GameObject gPlayer){
		Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
		Destroy(cPlayer);
	}

    protected override GameObject InstantiateCanvas(string canvasName)
    {
        GameObject gCanvas = LoadResource(canvasName);
        gCanvas.transform.position = new Vector3(0, 0, 0);
        return gCanvas;
    }

    protected override GameObject InstantiatePlayer(string playerName, Vector3 position)
    {
		GameObject gPlayer = LoadResource(playerName);
		gPlayer.transform.position = position;
        return gPlayer;
    }

    protected override Player_Base AddComponent_RPC(GameObject gPlayer){
        Player_Base cPlayer = gPlayer.AddComponent<Player_CpuMode>();
        return cPlayer;
    }
	protected override void DestroySync(GameObject g){
		Destroy(g);
	}

    protected override string GetCanvasName(){
        return "Canvas";
    }

    protected override string GetPlayerName(){
        return "Player";
	}
	public override void SetupSlider_RPC(GameObject gCanvas,GameObject gPlayer,int iPlayerNo)
	{
        SetupSliderCommon(gCanvas, gPlayer, iPlayerNo);
    }

}
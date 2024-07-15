using UnityEngine;
using UnityEngine.UI;
public class Field_Player_CpuMode : Field_Player_Base {
    protected override Player_Base AddComponent(GameObject gPlayer){
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
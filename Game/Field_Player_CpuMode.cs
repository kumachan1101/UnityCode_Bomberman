using UnityEngine;
using UnityEngine.UI;
public class Field_Player_CpuMode : Field_Player_Base {
/*
    public override void AddDummyPlayer(int playercnt, Vector3 v3){
		PreAddDummyPlayer();
		string canvasName = "";
		string playerName = "";
        GetPlayerDummyNames(playercnt, ref canvasName, ref playerName);
		GameObject gCanvas = InstantiateCanvas(canvasName);

        GameObject gPlayer = InstantiatePlayer(playerName, v3);
        //gPlayer.name = "PlayerDummy" + playercnt;

        // gPlayerにアタッチされているPlayerスクリプトを取得
        Player_Base playerComponent = gPlayer.GetComponent<Player_Base>();
        if (playerComponent != null)
        {
            // Playerスクリプトを削除
			// このタイミングですぐ消えるわけではないため、直後のコンポーネントを取得してもPlayerスクリプトが取得できてしまう。
			// 取得されてしまったPlayerスクリプトにスライダーを設定してしまうと、Destroyが動作したタイミングでスライダーが消える。
			StartCoroutine(DestroyComponentAndWait(playerComponent, gCanvas, gPlayer));
            //Destroy(playerComponent);
        }

        //CPUモードに切り替え
        Player_Base cPlayer = AddComponent(gPlayer);
        //cPlayer.MaterialType = "BomMaterial"+playercnt;
		//Debug.Log(gameObject.GetComponent<Field_Player_Base>());
        //cPlayer.SetPlayerSetting(GetPlayerCnt());
		//SetupSlider_RPC(gCanvas, gPlayer,GetPlayerCnt());
    }
*/
	protected override bool PreAddDummyPlayer(){
		AddPlayerCnt();
		return true;
	}

	protected override void PlayerDestroy(GameObject gPlayer){
		Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
		Destroy(cPlayer);
	}


/*
    public override void SpawnPlayerObjects(int iPlayerNo)
    {
		string canvasName = "";
		string playerName = "";
		GetPlayerNames(iPlayerNo, ref canvasName, ref playerName);
		
		GameObject gCanvas = InstantiateCanvas(canvasName);
		GameObject gPlayer = InstantiatePlayer(playerName, GetPlayerPosition(GetIndex(),iPlayerNo-1));

		//Slider cSlider = gCanvas.GetComponentInChildren<Slider>(); // Canvasの子要素からSliderを取得します。
		//SetPower(cSlider);
		//SetPlayerCnt(iPlayerNo); 
		//InitializePlayer(gPlayer, iPlayerNo);
		SetupSlider_RPC(gCanvas, gPlayer, iPlayerNo);

    }
*/
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
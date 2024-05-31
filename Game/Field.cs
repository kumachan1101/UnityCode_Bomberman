using UnityEngine;
using Photon.Pun;
using System.Reflection;

public class Field : Field_Base {

    protected Vector3[] v3PlayerPos1 = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, 1)
    };

    protected override void CPUmodeInit(){
    }


    protected override string GetCanvasName(){
        return "CanvasOnline";
    }

    protected override string GetPlayerName(){
        return "PlayerOnline";
	}



    protected override void SpawnPlayerObjects(int playerCount)
    {
        m_playerCount = playerCount;
        for (int i = 1; i <= playerCount; i++)
        {
            string canvasName = "Canvas" + i;
            GameObject gCanvas = LoadResource(canvasName);
            Vector3 v3PwrGage = new Vector3(0, 0, 0);
            gCanvas.transform.position = v3PwrGage;

            string PlayerName = "Player" + i;
            GameObject gPlayer = LoadResource(PlayerName);
            SetupPlayer(gPlayer, i, gCanvas);
        }
    }


    protected override int GetIndex(){
        return 1;
    }


    protected override void ClearBrokenList_RPC(){
        photonView.RPC(nameof(ClearBrokenList), RpcTarget.All);
    }

    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        photonView.RPC(nameof(InsBrokenBlock), RpcTarget.All, x, y, z);
    }


    protected override void InsObjMove_RPC(int x, int y, int z, Direction randomDirection){
        photonView.RPC(nameof(InsObjMove), RpcTarget.All, x, y, z, randomDirection);
    }


    protected override string GetBomMaterial(Vector3 target, int index)
    {
        target.y += 1;

        // v3PlayerPosの各要素と比較
        for (int i = 0; i < GetArrayLength(index); i++)
        {
            if (GetPlayerPosition(index,i) == target)
            {
                // 一致する要素が見つかった場合、該当する文字列を返す
                return "BomMaterial" + (i + 1);
            }
        }

        return "InvalidMaterial";
    }



    protected override void GameTransision()
    {
    }


    protected override void Rainbow_RPC(string sMaterialType){
        photonView.RPC(nameof(Rainbow), RpcTarget.All, sMaterialType);
    }

    public override void AddDummyPlayer(int playercnt, Vector3 v3){
        m_playerCount++;
		ItemControl cItemControl = GameObject.Find("ItemControl").GetComponent<ItemControl>();
		if(false == cItemControl.IsMaster()){
			return;
		}

        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        string canvasName = GetCanvasName() + playercnt;
		GameObject gCanvas = PhotonNetwork.Instantiate(canvasName, v3PwrGage, Quaternion.identity);

        GameObject gPlayer = PhotonNetwork.Instantiate(GetPlayerName() + "Dummy"+ playercnt, v3, Quaternion.identity);
		//gPlayer.name = "PlayerDummy" + playercnt;
        //gPlayer.tag = "Player_DummyMode";

        // gPlayerにアタッチされているPlayerスクリプトを取得
		/*
        Player_Base playerComponent = gPlayer.GetComponent<Player_Base>();
        if (playerComponent != null)
        {
            // Playerスクリプトを削除
            Destroy(playerComponent);
        }
		*/
        //CPUモードに切り替え
        //Player_Base cPlayer = AddComponent(gPlayer);

        //cPlayer.MaterialType = "BomMaterial"+playercnt;
        
        //cPlayer.SetViewID(GetComponent<PhotonView>().ViewID);
        //cPlayer.SetSlider(gCanvas);
        //SetupSlider(m_playerCount, gCanvas);

    }
    protected override Player_Base AddComponent(GameObject gPlayer){
        Player_Base cPlayer = gPlayer.AddComponent<Player_Online_Dummy>();
        return cPlayer;
    }


}
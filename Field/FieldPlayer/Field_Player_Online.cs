using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
public class Field_Player_Online : Field_Player_Base {

	protected Vector3[] v3PlayerPos1;
    public override void SetPlayerPositions()
    {
        // GameManager.xmax と GameManager.zmax の値がここで取得可能であると仮定
        int xmax = GameManager.xmax;
        int zmax = GameManager.zmax;


        v3PlayerPos1 = new Vector3[]
        {
            new Vector3(2, 0.5f, 2),
            new Vector3(xmax - 3, 0.5f, zmax - 3),
            new Vector3(2, 0.5f, zmax - 3),
            new Vector3(xmax - 3, 0.5f, 2)
        };
    }


    protected override string GetCanvasName(){
        return "CanvasOnline";
    }
/*
    public override string GetPlayerName(){
        //return "PlayerOnline";
        return playername;
	}
*/
/*
	protected override void PlayerDestroy(GameObject gPlayer){
		photonView.RPC(nameof(OnlinePlayerDestroy), RpcTarget.All, gPlayer.GetComponent<PhotonView>().ViewID);
		//OnlinePlayerDestroy(gPlayer.GetComponent<PhotonView>().ViewID);
	}

	[PunRPC]
	public void OnlinePlayerDestroy(int iViewIDPlayer)
	{
		PhotonView ViewPlayer = PhotonView.Find(iViewIDPlayer);
		if (ViewPlayer == null)
		{
			Debug.Log("ViewFind Error");
			return;
		}
		GameObject gPlayer = ViewPlayer.gameObject;
		if (gPlayer == null)
		{
			Debug.Log("GameObject Error");
			return;
		}
		PlayerDestroyComponent(gPlayer);
	}
*/

    protected GameObject InstantiateCanvas(string canvasName,int iPlayerCnt, int iPlayerNo)
    {
        return PhotonNetwork.Instantiate(canvasName, new Vector3(0, 0, 0), Quaternion.identity, 0, new object[] { iPlayerCnt, iPlayerNo });
    }

    protected GameObject InstantiatePlayer(string playerName, Vector3 position, int iCanvasInsID, int iPlayerNo, string script)
    {
        return PhotonNetwork.Instantiate(playerName, position, Quaternion.identity,0, new object[] { iCanvasInsID, iPlayerNo ,script});
    }

    public override int GetIndex(){
        return 1;
    }

	protected override bool PreAddDummyPlayer(){
		AddPlayerCnt();
		return GetComponent<PhotonView>().IsMine;
	}

    protected void AddComponent_RPC(GameObject gPlayer, Type componentType)
    {
        Debug.Log("AddComponent_RPC");
        string typeName = componentType.AssemblyQualifiedName; // 型情報を文字列に変換
        photonView.RPC(nameof(AddComponent), RpcTarget.All, gPlayer.GetComponent<PhotonView>().ViewID, typeName);
        return;
    }

    [PunRPC]
    public void AddComponent(int iViewID, string typeName)
    {
        PhotonView viewPlayer = PhotonView.Find(iViewID);
        if (viewPlayer == null)
        {
            Debug.LogError("ViewFind Error");
            return;
        }

        GameObject gPlayer = viewPlayer.gameObject;
        if (gPlayer == null)
        {
            Debug.LogError("GameObject Error");
            return;
        }

        // 渡された文字列から型を復元
        Type componentType = Type.GetType(typeName);
        if (componentType == null)
        {
            Debug.LogError($"Failed to get Type from name: {typeName}");
            return;
        }

        // 動的にコンポーネントを追加
        Player_Base cPlayer = gPlayer.AddComponent(componentType) as Player_Base;
        if (cPlayer == null)
        {
            Debug.LogError($"Failed to add component: {componentType.Name}");
        }
        Debug.Log(cPlayer);
        cPlayer.AddPlayerComponent();
        return;
    }

    public override string GetBomMaterial(Vector3 target, int index)
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

    public override void AddDummyPlayer(int iPlayerNo, Vector3 v3)
    {
        if(false == IsAddDummyPlayer(iPlayerNo)){
            return;
        }
        string canvasName = "";
        string playerName = "";
        GetPlayerNames(iPlayerNo, ref canvasName, ref playerName);
		
        GameObject gCanvas = InstantiateCanvas("CanvasPowerGage",GetPlayerCnt(), iPlayerNo);
        GameObject gPlayer = InstantiatePlayer("Player", v3, gCanvas.GetComponent<PhotonView>().ViewID, iPlayerNo,"Player_Online_Dummy");
    }


    public override void SpawnPlayerObjects(int iPlayerNo)
    {
        string canvasName = "";
        string playerName = "";
        GetPlayerNames(iPlayerNo, ref canvasName, ref playerName);

        GameObject gCanvas = InstantiateCanvas("CanvasPowerGage",GetPlayerCnt(), iPlayerNo);
        //Debug.Log(gCanvas.GetComponent<PhotonView>().ViewID);
        GameObject gPlayer = InstantiatePlayer("Player", GetPlayerPosition(GetIndex(), iPlayerNo - 1),gCanvas.GetComponent<PhotonView>().ViewID, iPlayerNo,"Player_Online");
        SetPlayerName(gPlayer.name);
    }
}
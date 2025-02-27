using UnityEngine;
using Photon.Pun;
using System;

public class PlayerPowerManager_Online: PlayerPowerManager
{
    public override int GetPower(){
        return 10;
    }

}
public class PlayerPositionManager_Online: PlayerPositionManager
{
    public override void SetPlayerPositions()
    {
        // GameManager.xmax と GameManager.zmax の値がここで取得可能であると仮定
        int xmax = GameManager.xmax;
        int zmax = GameManager.zmax;


        playerPositions = new Vector3[]
        {
            new Vector3(2, 0.5f, 2),
            new Vector3(xmax - 3, 0.5f, zmax - 3),
            new Vector3(2, 0.5f, zmax - 3),
            new Vector3(xmax - 3, 0.5f, 2)
        };
    }
}

public class PlayerSpawnManager_Online : PlayerSpawnManager {

    private bool IsAddDummyPlayer(int iPlayerNo){
        int iPlayerCnt = Library_Base.CountObjectsWithName("Player"+iPlayerNo);
        //Debug.Log(iPlayerCnt);
        if(iPlayerCnt >= 5){
            return false;
        }
        bool bIsMine = PreSpawnDummyPlayer();
        if (!bIsMine)
        {
            return false;
        }
        return true;
    }

    protected override void PlayerAddConponent (){
        cPlayerPositionManager = gameObject.AddComponent<PlayerPositionManager_Online>();
        gameObject.AddComponent<PlayerPowerManager_Online>();
    }

    protected GameObject InstantiateCanvas(string canvasName,int iPlayerCnt, int iPlayerNo)
    {
        return PhotonNetwork.Instantiate(canvasName, new Vector3(0, 0, 0), Quaternion.identity, 0, new object[] { iPlayerCnt, iPlayerNo });
    }

    protected GameObject InstantiatePlayer(string playerName, Vector3 position, int iCanvasInsID, int iPlayerNo, string script)
    {
        return PhotonNetwork.Instantiate(playerName, position, Quaternion.identity,0, new object[] { iCanvasInsID, iPlayerNo ,script});
    }

	protected override bool PreSpawnDummyPlayer(){
		cPlayerCountManager.AddPlayerCount();
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

    public override void SpawnDummyPlayer(int iPlayerNo, Vector3 v3)
    {
        if(false == IsAddDummyPlayer(iPlayerNo)){
            return;
        }
        string canvasName = "";
        string playerName = "";
        cPlayerNameManager.GetPlayerNames(iPlayerNo, ref canvasName, ref playerName);
		
        GameObject gCanvas = InstantiateCanvas("CanvasPowerGage",cPlayerCountManager.GetPlayerCount(), iPlayerNo);
        GameObject gPlayer = InstantiatePlayer("Player", v3, gCanvas.GetComponent<PhotonView>().ViewID, iPlayerNo,"Player_Online_Dummy");
    }


    public override void SpawnPlayer(int iPlayerNo)
    {
        string canvasName = "";
        string playerName = "";
        cPlayerNameManager.GetPlayerNames(iPlayerNo, ref canvasName, ref playerName);

        GameObject gCanvas = InstantiateCanvas("CanvasPowerGage",cPlayerCountManager.GetPlayerCount(), iPlayerNo);
        //Debug.Log(gCanvas.GetComponent<PhotonView>().ViewID);
        GameObject gPlayer = InstantiatePlayer("Player", cPlayerPositionManager.GetPlayerPosition(iPlayerNo - 1),gCanvas.GetComponent<PhotonView>().ViewID, iPlayerNo,"Player_Online");
        cPlayerNameManager.SetPlayerName(gPlayer.name);
    }
}
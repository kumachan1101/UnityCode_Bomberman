using UnityEngine;
using Photon.Pun;
using System.Reflection;
using UnityEngine.UI;

public class Field_Player_Online : Field_Player_Base {

    protected Vector3[] v3PlayerPos1 = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(GameManager.xmax-2, 1, GameManager.zmax-2),
        new Vector3(1, 1, GameManager.zmax-2),
        new Vector3(GameManager.xmax-2, 1, 1)
    };

    public override void SetPlayerPositions()
    {
        // GameManager.xmax と GameManager.zmax の値がここで取得可能であると仮定
        int xmax = GameManager.xmax;
        int zmax = GameManager.zmax;

        v3PlayerPos1 = new Vector3[]
        {
            new Vector3(1, 1, 1),
            new Vector3(xmax - 2, 1, zmax - 2),
            new Vector3(1, 1, zmax - 2),
            new Vector3(xmax - 2, 1, 1)
        };
    }


    protected override string GetCanvasName(){
        return "CanvasOnline";
    }

    protected override string GetPlayerName(){
        return "PlayerOnline";
	}

	protected override void DestroySync(GameObject g){
		PhotonNetwork.Destroy(g.GetComponent<PhotonView>());
	}

    // 引数playerCountで受け取り、キャンバスとプレイヤーオブジェクトを生成する関数
    public override void SpawnPlayerObjects(int iPlayerNo)
    {
        string canvasName = "CanvasOnline" + iPlayerNo;
        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        GameObject gCanvas = PhotonNetwork.Instantiate(canvasName, v3PwrGage, Quaternion.identity);

		string playerName = "PlayerOnline" + iPlayerNo;
        Vector3 v3PlayerPos = GetPlayerPosition(1, iPlayerNo-1);
        GameObject gPlayer = PhotonNetwork.Instantiate(playerName, v3PlayerPos, Quaternion.identity);
        SetName("PlayerOnline" + iPlayerNo + "(Clone)");
		//m_iPlayerNo = PhotonNetwork.PlayerList.Length;

        SetupSlider_RPC(gCanvas, gPlayer, iPlayerNo);

    }



    public override int GetIndex(){
        return 1;
    }
	public override void SetupSlider_RPC(GameObject gCanvas,GameObject gPlayer, int iPlayerNo)
	{
        photonView.RPC(nameof(SetupSlider), RpcTarget.All, gCanvas.GetComponent<PhotonView>().ViewID,  gPlayer.GetComponent<PhotonView>().ViewID,iPlayerNo);
    }

	[PunRPC]
	public void SetupSlider(int iViewIDCanvas, int iViewIDPlayer, int iPlayerNo)
	{
		PhotonView ViewCanvas = PhotonView.Find(iViewIDCanvas);
		PhotonView ViewPlayer = PhotonView.Find(iViewIDPlayer);
		if (ViewCanvas == null || ViewPlayer == null)
		{
			Debug.Log("ViewFind Error");
			return;
		}
		GameObject gCanvas = ViewCanvas.gameObject;
		GameObject gPlayer = ViewPlayer.gameObject;
		if (gCanvas == null || gPlayer == null)
		{
			Debug.Log("GameObject Error");
			return;
		}
		m_playerCount = PhotonNetwork.PlayerList.Length;;
		SetupSliderCommon(gCanvas, gPlayer, iPlayerNo);
	}

    public override void AddDummyPlayer(int playercnt, Vector3 v3){
        m_playerCount++;
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        string canvasName = GetCanvasName() + playercnt;
		GameObject gCanvas = PhotonNetwork.Instantiate(canvasName, v3PwrGage, Quaternion.identity);
        GameObject gPlayer = PhotonNetwork.Instantiate(GetPlayerName() + "Dummy"+ playercnt, v3, Quaternion.identity);
        SetupSlider_RPC(gCanvas, gPlayer, m_playerCount);

    }

    protected override Player_Base AddComponent(GameObject gPlayer){
        Player_Base cPlayer = gPlayer.AddComponent<Player_Online_Dummy>();
        return cPlayer;
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

}
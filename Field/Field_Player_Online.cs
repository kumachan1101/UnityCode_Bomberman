using UnityEngine;
using Photon.Pun;
using System.Collections;
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

    protected override string GetPlayerName(){
        return "PlayerOnline";
	}


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
		Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
		Debug.Log(cPlayer);
		Destroy(cPlayer);
	}


    protected override GameObject InstantiateCanvas(string canvasName)
    {
        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        return PhotonNetwork.Instantiate(canvasName, v3PwrGage, Quaternion.identity);
    }

    protected override GameObject InstantiatePlayer(string playerName, Vector3 position)
    {
        return PhotonNetwork.Instantiate(playerName, position, Quaternion.identity);
    }

    protected override void GetPlayerNames(int iPlayerNo, ref string canvasName, ref string playerName)
    {
        canvasName = "CanvasOnline" + iPlayerNo;
        playerName = "PlayerOnline" + iPlayerNo;
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
		//OnlineSetupSliderCommon(gCanvas, gPlayer, iPlayerNo);
		SetupSliderCommon(gCanvas, gPlayer, iPlayerNo);
	}


	private void OnlineSetupSliderCommon(GameObject gCanvas, GameObject gPlayer, int iPlayerNo){
		StartCoroutine(OnlineSetupSliderCommonCoroutine(gCanvas, gPlayer, iPlayerNo));
	}

	private IEnumerator OnlineSetupSliderCommonCoroutine(GameObject gCanvas, GameObject gPlayer, int iPlayerNo){
		while(true){
			Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
			if (cPlayer != null)
			{
				SetupSliderCommon(gCanvas, gPlayer, iPlayerNo);
				break;
			}
			else{
				yield return new WaitForEndOfFrame(); // 現在のフレームの終了まで待つ
			}
		}
	}

	protected override bool PreAddDummyPlayer(){
		AddPlayerCnt();
		return GetComponent<PhotonView>().IsMine;
	}

    protected override Player_Base AddComponent_RPC(GameObject gPlayer){
		photonView.RPC(nameof(AddComponent), RpcTarget.All, gPlayer.GetComponent<PhotonView>().ViewID);
        return null;
    }

	[PunRPC]
    protected Player_Base AddComponent(int iViewID){
		PhotonView ViewPlayer = PhotonView.Find(iViewID);
		if (ViewPlayer == null)
		{
			Debug.Log("ViewFind Error");
			return null;
		}
		GameObject gPlayer = ViewPlayer.gameObject;
		if (gPlayer == null)
		{
			Debug.Log("GameObject Error");
			return null;
		}

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
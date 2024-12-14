using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine;

public class ButtonClickScript_Online : ButtonClickScript
{
/*
    override public void LoadGameScene(){
		DestroyAllPhotonViews();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("GameTitle");
    }
	[PunRPC]
    public void LoadGameScene_RPC()
    {
		DestroyAllPhotonViews();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("GameTitle");
    }


*/

    void Start(){
    }

    override public void LoadGameScene()
    {
        Debug.Log("Online LoadGameScene");
		PhotonView cPhotonView = GetComponent<PhotonView>();
        cPhotonView.TransferOwnership(PhotonNetwork.MasterClient);
        PhotonNetwork.LoadLevel("GameTitle");
		//cPhotonView.RPC(nameof(LoadGameScene_RPC), RpcTarget.All);
    }
	[PunRPC]
    public void LoadGameScene_RPC()
    {
        SceneManager.LoadScene("GameTitle");
        //PhotonNetwork.LoadLevel("GameTitle");
    }


	public void DestroyAllPhotonViews()
	{
		foreach (PhotonView view in FindObjectsOfType<PhotonView>())
		{
			if (view.IsMine)
			{
				PhotonNetwork.Destroy(view.gameObject);
			}
		}
	}


}
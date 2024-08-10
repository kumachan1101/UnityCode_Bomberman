using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ButtonClickScript : MonoBehaviourPunCallbacks
{

    public void LoadGameScene()
    {
		DestroyAllPhotonViews();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("GameTitle");
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
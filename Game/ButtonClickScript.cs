using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ButtonClickScript : MonoBehaviourPunCallbacks
{

    public void LoadGameScene()
    {
        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("GameTitle");
        //SceneManager.LoadScene("GameTitle");
    }
}
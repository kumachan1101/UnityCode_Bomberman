using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OnlineLobby : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        ReqJoinLobby();
    }

    public virtual void ReqJoinLobby(){}

    protected void BaseJoinLobby(TypedLobby lobby)
    {
        PhotonNetwork.JoinLobby(lobby);
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

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OnlineLobby1 : OnlineLobby
{
    private TypedLobby lobby1 = new TypedLobby("Lobby1", LobbyType.Default);

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void ReqJoinLobby(){
        BaseJoinLobby(lobby1);
    }

}

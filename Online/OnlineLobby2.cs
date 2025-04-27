using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OnlineLobby2 : OnlineLobby
{
    private TypedLobby lobby2 = new TypedLobby("Lobby2", LobbyType.Default);

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void ReqJoinLobby(){
        BaseJoinLobby(lobby2);
    }

}

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MatchmakingView_Tower : MatchmakingView
{
	void Awake() {
		cRoomListUI = this.gameObject.AddComponent<RoomListUI_Tower>();
        gField = GameObject.Find("Field");
	}

    protected override void HandleTowerObjects(int playerCount) {
        TowerSpawnManager cBlockCreateManager = gField.GetComponent<TowerSpawnManager_Online>();
        cBlockCreateManager.init(playerCount);
/*
        for(int i = 0; i < playerCount; i++) {
            cBlockCreateManager.SetPositions();
            cBlockCreateManager.SpawnTowerObjects(i);
        }
        cBlockCreateManager.CreateButtonCanvas();
*/
    }
}



public class RoomListUI_Tower : RoomListUI
{
    protected override string GetRoomName(){
        return "Tower";
    }


}
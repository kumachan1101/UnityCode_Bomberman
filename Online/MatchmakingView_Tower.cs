using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MatchmakingView_Tower : MatchmakingView
{

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
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MatchmakingView_Tower : MatchmakingView
{

    protected override void HandleTowerObjects(int playerCount, GameObject gField) {
        BlockCreateManager_Tower cBlockCreateManager = gField.GetComponent<BlockCreateManager_TowerOnline>();
        for(int i = 0; i < playerCount; i++) {
            cBlockCreateManager.SetPositions();
            cBlockCreateManager.SpawnTowerObjects(i);
        }
        cBlockCreateManager.CreateButtonCanvas();
    }

}
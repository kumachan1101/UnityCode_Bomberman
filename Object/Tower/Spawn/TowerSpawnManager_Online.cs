using UnityEngine;
using Photon.Pun;
public class TowerSpawnManager_Online : TowerSpawnManager
{
    public override void SpawnTowerObjects(int index)
    {
        if (!IsValidTowerIndex(index))
        {
            Debug.LogError("Invalid index for tower position: " + index);
            return;
        }
        photonView.RPC(nameof(SetpUpSpawnTowerObjects), RpcTarget.All, index);
    }
    
    

}

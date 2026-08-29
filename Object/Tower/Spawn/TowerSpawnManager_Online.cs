using UnityEngine;
using Photon.Pun;
public class TowerSpawnManager_Online : TowerSpawnManager
{
    void Start()
    {
        // overrideして処理しないようにするため、この関数は削除できない
    }

    public override void SpawnTowerObjects(int index)
    {
        if (!IsValidTowerIndex(index))
        {
            Debug.LogError("Invalid index for tower position: " + index);
            return;
        }
        photonView.RPC(nameof(SetpUpSpawnTowerObjects), RpcTarget.All, index);
    }

    public override bool TrySpendRevivalPower(int playerNo, int powerCost)
    {
        if (!CanSpendRevivalPower(playerNo, powerCost)) return false;
        photonView.RPC(nameof(SpendRevivalPowerForAll), RpcTarget.All,
            playerNo, powerCost);
        return true;
    }

    [PunRPC]
    private void SpendRevivalPowerForAll(int playerNo, int powerCost)
    {
        base.TrySpendRevivalPower(playerNo, powerCost);
    }
    
    

}

using UnityEngine;
using Photon.Pun;
public class BlockCreateManager_TowerOnline : BlockCreateManager_Tower
{
    void Start()
    {
        InitComponent();
    }
/*
    protected override void SetupTowerCanvasIntegration(GameObject tower, GameObject canvas, int index)
    {
        PowerGageIF cPowerGageIF = tower.AddComponent<PowerGageIF_TowerOnline>();
        cPowerGageIF.SetCanvasInsID(canvas.GetInstanceID());
    }
*/
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

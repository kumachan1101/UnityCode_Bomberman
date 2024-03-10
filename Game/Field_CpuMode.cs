using UnityEngine;

public class Field_CpuMode : Field {
    protected override void CPUmodeInit(){
        int playerCount = 4;
        GameObject gItemControl = GameObject.Find("ItemControl");
        gItemControl.GetComponent<ItemControl_CpuMode>().SetMaster();

        CreateBrokenBlock();
        SpawnPlayerObjects(playerCount);

        PlayerName = "Player1(Clone)";

    }

    protected override void ClearBrokenList_RPC(){
        ClearBrokenList();
    }


    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        InsBrokenBlock(x, y, z);
    }

}
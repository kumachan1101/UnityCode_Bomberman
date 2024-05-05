using UnityEngine;
using PlayerActionName;
using PowerGageName;
public class Player_CpuMode : Player
{

    public override void UpdateKey(){
        DropBom();
    }


    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction_CpuMode(ref rigidBody, ref myTransform, ref animator, ref cField, iViewID);
    }


    protected override bool IsAvairable(){
        //Debug.Log(iViewID);
        if(iViewID == 9999){
            return false;
        }
        return true;
    }

    protected override void DropBom_BomControl(GameObject gBomControl, Vector3 v3, int iViewID){
        Vector3 direction = myTransform.forward;
        gBomControl.GetComponent<BomControl_CpuMode>().DropBom(ref cPlayerBom, v3, iViewID, direction);
    }
    protected override Player GetComponent(){
        return this.gameObject.GetComponent<Player_CpuMode>();
    }
}

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

}

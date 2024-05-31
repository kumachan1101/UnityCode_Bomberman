using UnityEngine;
using PlayerActionName;
using PowerGageName;
using System.Collections.Generic;

public class Player_CpuMode : Player_Base
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

/*
    public override void SetViewID(int iParamViewID){
        //Debug.Log(iParamViewID);
        iViewID = iParamViewID;
        CreatePlayerBom();
        cPlayerBom.SetViewID(iViewID);
        cPlayerBom.SetMaterialType(MaterialType);

        rigidBody = GetComponent<Rigidbody> ();
        myTransform = transform;
        animator = myTransform.Find ("PlayerModel").GetComponent<Animator> ();
        cField = GetField();

        CreatePlayerAction();
        cPlayerAction.SetMaterialType(MaterialType);
    }
*/
}

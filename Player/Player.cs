using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using UnityEngine.UI;
using PowerGageName;
using PlayerActionName;
using PlayerBomName;
using System.Collections.Generic;

public class Player : Player_Base
{

    public override void SetSlider(GameObject gCanvas){
        cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
    }

    public override void UpdateKey(){
        if (Input.GetKey(KeyCode.Return)) {
             if (pushFlag == false){
                //Debug.Log($"{iViewID} is Return");
                pushFlag = true;
                DropBom();
                //AttackExplosion();
             }
        }
        else{
            pushFlag = false;
        }
    }

    protected override bool IsAvairable(){
        if(iViewID == 9999){
            return false;
        }
        return true;
    }





    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction(ref rigidBody, ref myTransform, ref animator, ref cField, iViewID);
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

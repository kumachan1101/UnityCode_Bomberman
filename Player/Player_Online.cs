using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using UnityEngine.UI;
using PowerGageName;
using PlayerActionName;
using PlayerBomName;
using System.Text.RegularExpressions;
public class Player_Online : Player_Base
{
    void Awake ()
    {
        string myName = gameObject.name;

        Match match = Regex.Match(myName, @"\d+");
        string numberString = match.Value;
        string newCanvasName = "CanvasOnline" + numberString + "(Clone)";
        //Debug.Log(newCanvasName);
        GameObject gCanvas = GameObject.Find(newCanvasName);
        SetSlider(gCanvas);
        SetViewID(GetComponent<PhotonView>().ViewID);
        //Debug.Log("ViewID:"+iViewID);
    }

    void Update ()
    {
        if(false == GetComponent<PhotonView>().IsMine){
            return;
        }
        UpdatePlayer();
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

    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction(ref rigidBody, ref myTransform, ref animator, ref cField, iViewID);
    }

}

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
        cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage_CpuMode>();
    }


    protected override bool IsAvairable(){
        if(iViewID == 9999){
            return false;
        }
        return true;
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

    protected override void DropBom_BomControl(GameObject gBomControl, Vector3 v3, int iViewID){
        Vector3 direction = myTransform.forward;
        gBomControl.GetComponent<BomControl_CpuMode>().DropBom(ref cPlayerBom, v3, iViewID, direction);
    }

    public override void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Explosion"))
        {
            string materialName = other.GetComponent<Renderer>().material.name.Replace(" (Instance)", "");
            if(MaterialType != materialName){
                int iDamage = other.GetComponent<Explosion>().GetDamage();
                Player_Base cPlayer = GetComponent();
                //Debug.Log(cPlayer);
                cPlayer.cPowerGage.SetDamage(iDamage);
    
                if(cPlayer.cPowerGage.IsDead()){
                    string tag = this.gameObject.tag;
                    //Dead(tag);
                    Destroy(this.gameObject);
                }
            }
        }
    }

    protected override Player_Base GetComponent(){
        return this.gameObject.GetComponent<Player>();
    }


    protected override Field GetField(){
        return GameObject.Find("Field").GetComponent<Field_CpuMode>();
    }

    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction(ref rigidBody, ref myTransform, ref animator, ref cField, iViewID);
    }

}
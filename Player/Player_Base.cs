using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using UnityEngine.UI;
using PowerGageName;
using PlayerActionName;
using PlayerBomName;
using System.Collections.Generic;

public class Player_Base : MonoBehaviourPunCallbacks
{

    public string MaterialType;
    public int playerNumber = 1;
    protected Rigidbody rigidBody;
    protected Transform myTransform;
    protected Animator animator;
    protected Field_Base cField;
    protected bool pushFlag = false;
    public int iViewID = 9999;
    public PowerGageName.PowerGage cPowerGage;
    protected PlayerBomName.PlayerBom cPlayerBom;
    protected PlayerAction cPlayerAction;
    protected Library cLibrary;
    public virtual void SetSlider(GameObject gCanvas){
		cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
	}

    protected virtual bool IsAvairable(){
        return false;
    }

    public virtual void UpdateKey(){}

    protected virtual void DropBom_BomControl(GameObject gBomControl, Vector3 v3, int iViewID){
        Vector3 direction = myTransform.forward;
        gBomControl.GetComponent<BomControl>().DropBom(ref cPlayerBom, v3, iViewID, direction);
	}


    protected virtual Player_Base GetComponent(){
		return this.gameObject.GetComponent<Player_Base>();
    }

    protected virtual Field_Base GetField(){
		return GameObject.Find("Field").GetComponent<Field_Base>();
    }

    protected virtual void CreatePlayerAction(){}

    public virtual void SetViewID(int iParamViewID){
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

    // Update is called once per frame
    void Update ()
    {
        if(false == IsAvairable()){
            return;
        }
        UpdatePlayer();
    }

    protected void UpdatePlayer(){
        cPlayerAction.UpdateMovement();
        UpdateKey();
        cPlayerAction.UpdateButton();
    }


    public void DropBom(){
        if(null == cLibrary){
            cLibrary = GameObject.Find("Library").GetComponent<Library>();
        }
        Vector3 v3 = cLibrary.GetPos(transform.position);
        if(cPlayerBom.isAbalableBom(v3)){
            GameObject gBomControl = GameObject.Find("BomControl");
            DropBom_BomControl(gBomControl, v3, iViewID);
        }
    }

    private void AttackExplosion(){
        //Vector3 v3 = GetPos();
        //Vector3 direction = myTransform.forward;
        //DropBomと同じように実装しよう。
        GameObject gBomControl = GameObject.Find("BomControl");
        gBomControl.GetComponent<BomControl>().CancelInvokeAndCallExplosion();
    }

    public void OnTriggerEnter (Collider other)
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



    private void OnCollisionEnter(Collision collision){
        switch (collision.transform.name){
            case "Bom(Clone)":
            case "Bombigban(Clone)":
            case "BomExplode(Clone)":
                //Debug.Log(collision.transform.name);
                // ここに処理を記述
                break;
            case "Ground(Clone)":
                //Debug.Log("OnCollisionEnter:Ground");
                return;
            default:
                return;
        }

        Vector3 collisionDirectionTemp = Vector3.zero;
        Vector3 collisionDirection = Vector3.zero;

        foreach (ContactPoint contact in collision.contacts)
        {
            collisionDirectionTemp += contact.point - transform.position;
        }

        collisionDirectionTemp /= collision.contacts.Length;

        //Debug.Log("X :" + collisionDirectionTemp.x + "Z :" + collisionDirectionTemp.z);
        float threshold = 0.3f; // 閾値
        // x軸方向の判定
        if (Mathf.Abs(collisionDirectionTemp.x) > threshold)
        {
            collisionDirection.x = collisionDirectionTemp.x;
        }
        else if (Mathf.Abs(collisionDirectionTemp.z) > threshold)
        {
            collisionDirection.z = collisionDirectionTemp.z;
        }
        // Bomオブジェクトに方向を伝える
        collision.transform.GetComponent<Bom_Base>().SetMoveDirection(collisionDirection);
    }

    public void OnCollisionExit(Collision col) {
    }


    protected void CreatePlayerBom(){
        cPlayerBom = new PlayerBom();
    }



    public PlayerBom GetPlayerBom(){
        return cPlayerBom;
    }

    public PlayerAction GetPlayerAction(){
        return cPlayerAction;
    }

    public void Wall(){
        if(iViewID != GetComponent<PhotonView>().ViewID){
            return;
        }
        
        cField.SetBrokenTrriger(true);

        photonView.RPC(nameof(SetIsTrigger), RpcTarget.All, true);
        GetComponent<Collider>().isTrigger = false;        

    }

    [PunRPC]
    public void SetIsTrigger(bool bSet){
        GetComponent<Collider>().isTrigger = bSet;        
    }

    public void HeartUp(int iHeart){
        cPowerGage.HeartUp(iHeart);
    }


}
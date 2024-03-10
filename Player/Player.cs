using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using UnityEngine.UI;
using PowerGageName;
using PlayerActionName;
using PlayerBomName;

public class Player : MonoBehaviourPunCallbacks
{

    public string MaterialType;
    //Player parameters
    public int playerNumber = 1;
    //Indicates what player this is: P1 or P2

    //Can the player drop bombs?
    //Can the player move?

    //Cached components
    protected Rigidbody rigidBody;
    protected Transform myTransform;
    protected Animator animator;
    protected Field cField;

    private bool pushFlag = false;

    public int iViewID = 9999;


    public PowerGageName.PowerGage cPowerGage;
    //private Slider s;
    //private Vector3 SliderPos;

    protected PlayerBomName.PlayerBom cPlayerBom;


    protected PlayerAction cPlayerAction;

    public virtual void SetSlider(GameObject gCanvas){
        cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage_CpuMode>();
    }

    void Awake ()
    {  

    }

    // Use this for initialization
    void Start ()
    {
        /*
        if(iViewID != GetComponent<PhotonView>().ViewID){
            return;
        }
        */
        //Cache the attached components for better performance and less typing

        //cBomControl.SetBomSetting(MaterialType);
    }

    // Update is called once per frame
    void Update ()
    {
        //Debug.Log(this.gameObject.name);
        /*
        if(!GetComponent<PhotonView>().IsMine){
            return;
        }
        */
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

    protected virtual bool IsAvairable(){
        /*
        if(iViewID != GetComponent<PhotonView>().ViewID){
            return false;
        }
        */
        if(iViewID == 9999){
            return false;
        }
        return true;
    }

    public virtual void UpdateKey(){
        if (Input.GetKey(KeyCode.Return)) {
             if (pushFlag == false){
                //Debug.Log($"{iViewID} is Return");
                pushFlag = true;
                DropBom();
             }
        }
        else{
            pushFlag = false;
        }
    }

    public void DropBom(){
        GameObject gBomControl = GameObject.Find("BomControl");
        Vector3 v3 = GetPos();
        if(cPlayerBom.isAbalableBom(v3)){
            DropBom_BomControl(gBomControl, v3, iViewID);
        }
    }

    protected virtual void DropBom_BomControl(GameObject gBomControl, Vector3 v3, int iViewID){
        gBomControl.GetComponent<BomControl_CpuMode>().DropBom(ref cPlayerBom, v3, iViewID);
    }



    public virtual void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Explosion"))
        {
            string materialName = other.GetComponent<Renderer>().material.name.Replace(" (Instance)", "");
            if(MaterialType != materialName){
                int iDamage = other.GetComponent<Explosion>().GetDamage();
                string tag = this.gameObject.tag;
                Player cPlayer = null;
                if(tag == "Player"){
                    cPlayer = this.gameObject.GetComponent<Player>();
                }
                else if(tag == "Player_CpuMode"){
                    cPlayer = this.gameObject.GetComponent<Player_CpuMode>();
                }
                else if(tag == "Player_Online"){
                    cPlayer = this.gameObject.GetComponent<Player_Online>();
                }

                cPlayer.cPowerGage.SetDamage(iDamage);
                if(cPlayer.cPowerGage.IsDead()){
                    Destroy(this.gameObject);
                }

            }
        }
    }

    public void OnTriggerExit (Collider other)
    {
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
        float threshold = 0.4f; // 閾値
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
        collision.transform.GetComponent<BomName.Bom>().SetMoveDirection(collisionDirection);
    }

    public void OnCollisionExit(Collision col) {
    }


    public void SetViewID(int iParamViewID){
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

    protected virtual Field GetField(){
        return GameObject.Find("Field").GetComponent<Field_CpuMode>();
    }

    private void CreatePlayerBom(){
        cPlayerBom = new PlayerBom();
    }

    protected virtual void CreatePlayerAction(){
        cPlayerAction = new PlayerAction(ref rigidBody, ref myTransform, ref animator, ref cField, iViewID);
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



    public  void BtnMoveUp()
    {
        GameObject gPlayer = GameObject.Find("Player1(Clone)");
        Player cPlayer = gPlayer.GetComponent<Player>();
        cPlayer.GetPlayerAction().BtnMoveUp();
    }

    public  void BtnMoveDown()
    {
        GameObject gPlayer = GameObject.Find("Player1(Clone)");
        Player cPlayer = gPlayer.GetComponent<Player>();
        cPlayer.GetPlayerAction().BtnMoveDown();
    }

    public  void BtnMoveRight()
    {
        GameObject gPlayer = GameObject.Find("Player1(Clone)");
        Player cPlayer = gPlayer.GetComponent<Player>();
        cPlayer.GetPlayerAction().BtnMoveRight();
    }

    public  void BtnMoveLeft()
    {
        GameObject gPlayer = GameObject.Find("Player1(Clone)");
        Player cPlayer = gPlayer.GetComponent<Player>();
        cPlayer.GetPlayerAction().BtnMoveLeft();
    }

    public  void BtnDropBom()
    {
        GameObject gPlayer = GameObject.Find("Player1(Clone)");
        Player cPlayer = gPlayer.GetComponent<Player>();
        cPlayer.GetPlayerAction().BtnDropBom();
    }

    private Vector3 GetPos(){
        float x = Mathf.Round(transform.position.x);
        /*
        if(x % 2 == 1){
            x += 1;
        }
        */
        float y = 1;
        float z = Mathf.Round(transform.position.z);
        /*
        if(z % 2 == 1){
            z += 1;
        }
        */
        return new Vector3(x,y,z);
    }


}

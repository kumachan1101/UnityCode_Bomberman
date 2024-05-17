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
        if(!GetComponent<PhotonView>().IsMine){
            return;
        }
        UpdatePlayer();
    }

    protected override void DropBom_BomControl(GameObject gBomControl, Vector3 v3, int iViewID){
        gBomControl.GetComponent<BomControl_Online>().DropBom(ref cPlayerBom, v3, iViewID, myTransform.forward);
    }

    protected override Field GetField(){
        return GameObject.Find("Field").GetComponent<Field>();
    }

    public override void SetSlider(GameObject gCanvas){
        cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
    }

    protected override Player_Base GetComponent(){
        return this.gameObject.GetComponent<Player_Online>();
    }
}

using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using UnityEngine.UI;
using PowerGageName;
using PlayerActionName;
using PlayerBomName;
using System.Text.RegularExpressions;
public class Player_Online : Player
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
    }

    void Update ()
    {
        if(!GetComponent<PhotonView>().IsMine){
            return;
        }
        UpdatePlayer();
    }

    protected override void DropBom_BomControl(GameObject gBomControl, Vector3 v3, int iViewID){
        gBomControl.GetComponent<BomControl>().DropBom(ref cPlayerBom, v3, iViewID);
    }

    protected override Field GetField(){
        return GameObject.Find("Field").GetComponent<Field>();
    }

    public override void SetSlider(GameObject gCanvas){
        cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
    }

}

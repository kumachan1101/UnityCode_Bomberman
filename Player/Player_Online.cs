using UnityEngine;
using Photon.Pun;
using System.Text.RegularExpressions;
public class Player_Online : Player_Base
{
    void Awake ()
    {
		InitComponent();
        //string myName = gameObject.name;
        //Match match = Regex.Match(myName, @"\d+");
        //string numberString = match.Value;
        SetPlayerSetting(GetComponent<PhotonView>().ViewID);
    }

    protected override bool IsAvairable(){
		return GetComponent<PhotonView>().IsMine;
    }

    public override void UpdateKey(){
        if (Input.GetKey(KeyCode.Return)) {
             if (pushFlag == false){
                pushFlag = true;
                DropBom();
                //AttackExplosion();
             }
        }
        else{
            pushFlag = false;
        }
    }

    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction(ref rigidBody, ref myTransform);
    }

	protected override void DestroySync(GameObject g){
		PhotonView pv = g.GetComponent<PhotonView>();
		if (pv != null && pv.IsMine)
		{
			PhotonNetwork.Destroy(g);
		}
	}

}

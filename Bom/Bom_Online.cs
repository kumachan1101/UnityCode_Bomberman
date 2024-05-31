using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BomPosName;
using Photon.Pun;
using PlayerBomName;

public class Bom_Online : Bom_Base
{
	protected override bool IsExplosion(){
		return GetComponent<PhotonView>().IsMine;
	}

	protected override GameObject Instantiate_Explosion(Vector3 v3){
		string sExplosion = cMaterialMng.GetMaterialOfExplosion(sMaterialKind);
		GameObject g = PhotonNetwork.Instantiate(sExplosion, v3, Quaternion.identity);
		return g;
	}

}

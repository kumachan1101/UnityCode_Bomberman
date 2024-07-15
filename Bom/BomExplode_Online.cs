using Photon.Pun;
public class BomExplode_Online : BomExplode_Base
{
	protected override void init(){
		cInsManager = gameObject.AddComponent<InstanceManager_Online>();
        cInsManager.SetResource(sExplosion);
	}

	protected override bool IsExplosion(){
		return GetComponent<PhotonView>().IsMine;
	}

}

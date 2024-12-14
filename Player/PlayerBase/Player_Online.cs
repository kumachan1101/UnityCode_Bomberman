using Photon.Pun;
public class Player_Online : Player_Base
{
    public override void AddPlayerComponent(){
        this.gameObject.AddComponent<PlayerAction_Online>();
    }

	public override void DestroySync(){
		PhotonView pv = this.gameObject.GetComponent<PhotonView>();
		if (pv != null && pv.IsMine)
		{
			PhotonNetwork.Destroy(this.gameObject);
		}
	}

}

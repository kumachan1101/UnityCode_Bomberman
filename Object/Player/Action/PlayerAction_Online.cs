using Photon.Pun;
public class PlayerAction_Online : PlayerAction
{
    protected override bool IsAvailable(){
        return GetComponent<PhotonView>().IsMine;
    }
}

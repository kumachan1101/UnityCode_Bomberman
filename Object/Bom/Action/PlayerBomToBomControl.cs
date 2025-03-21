using UnityEngine;
public class PlayerBomToBomControl : MonoBehaviour
{
    BomControl cBomControl;
    PlayerBom cPlayerBom;

    GameManager cGameManager;

    public void Awake(){
        cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
        cPlayerBom = this.gameObject.AddComponent<PlayerBom>();
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void RequestDropBom(Vector3 position, Vector3 direction){
        if(false == cGameManager.GetSetUp()){
            return;
        }
		if(Library_Base.IsPositionOutOfBounds(position)){
			return;
		}
        if(false == cPlayerBom.IsBomAvailable(position)){
            return;
        }
        BomParameters bomParams = cPlayerBom.CreateBomParameters(position, direction);
        GameObject cBom = cBomControl.DropBom(bomParams);
        cPlayerBom.Add(cBom);
    }

}

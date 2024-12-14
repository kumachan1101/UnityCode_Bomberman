using UnityEngine;
public class PlayerBomToBomControl : MonoBehaviour
{
    BomControl cBomControl;
    PlayerBom cPlayerBom;

    public void Awake(){
        cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
        cPlayerBom = this.gameObject.AddComponent<PlayerBom>();
    }

    public void RequestDropBom(Vector3 position, Vector3 direction){
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

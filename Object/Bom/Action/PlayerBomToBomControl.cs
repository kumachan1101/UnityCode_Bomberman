using UnityEngine;
public class PlayerBomToBomControl : MonoBehaviour
{
    BomControl cBomControl;
    PlayerBom cPlayerBom;

    GameManager cGameManager;

    ItemControl cItemControl;

    public void Awake(){
        cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
        cPlayerBom = this.gameObject.AddComponent<PlayerBom>();
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cItemControl = GameObject.Find("ItemControl").GetComponent<ItemControl>();
    }

    public void RequestDropBom(){
        Vector3 position = Library_Base.GetPos(transform.position);
        if(false == CanDropBom(position)){
            return;
        }
        BomParameters bomParams = cPlayerBom.CreateBomParameters(position, transform.forward);
        GameObject cBom = cBomControl.DropBom(bomParams);
        cPlayerBom.Add(cBom);
    }

    bool CanDropBom(Vector3 position){
        if(false == cGameManager.GetSetUp()){
            return false;
        }
		if(Library_Base.IsPositionOutOfBounds(position)){
			return false;
		}
        if(false == cPlayerBom.IsBomAvailable(position)){
            return false;
        }
        if(null == cItemControl){
			return false;
		}
		if(cItemControl.IsItem(position)){
			return false;
		}
        if(null != Library_Base.GetGameObjectAtExactPositionWithName(position, "Explosion")){
			return false;
		}
        return true;
    }

}

using System;
using UnityEngine;
public class PlayerBomToBomControl : MonoBehaviour
{
    protected BomControl cBomControl;
    protected PlayerBom cPlayerBom;

    GameManager cGameManager;

    ItemControl cItemControl;
    BlockCreateManager cField;

    public void Awake(){
        cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
        cPlayerBom = this.gameObject.AddComponent<PlayerBom>();
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cItemControl = GameObject.Find("ItemControl").GetComponent<ItemControl>();
        GameObject fieldObject = GameObject.Find("Field");
        cField = fieldObject != null ? fieldObject.GetComponent<BlockCreateManager>() : null;
    }


    protected Action GetAction(){
        Action cAction = null;
        switch (cPlayerBom.GetBomAttack())
        {
            case BOM_ATTACK.BOM_ATTACK_MULTI:
                cAction = RequestDropBomMulti;
                break;
            case BOM_ATTACK.BOM_ATTACK_THROW:
            default:
                cAction = RequestDropBomNormal;
                break;
        }
        return cAction;
    }

    private void RequestDropBomNormal(){
        Vector3 position = Library_Base.GetPos(transform.position);
        if(false == CanDropBom(position)){
            return;
        }
        Vector3 direction = BomGridRules.GetCardinalDirection(transform.forward);
        BomParameters bomParams = cPlayerBom.CreateBomParameters(position, direction);
        GameObject cBom = cBomControl.DropBom(bomParams);
        cPlayerBom.Add(cBom);
    }
    private void RequestDropBomMulti(){
        Vector3 originCell = BomGridRules.ToCell(transform.position);
        Vector3 direction = BomGridRules.GetCardinalDirection(transform.forward);
        if (direction == Vector3.zero)
        {
            return;
        }

        int maximumCells = Mathf.Max(GameManager.xmax, GameManager.zmax);
        for (int distance = 1; distance <= maximumCells; distance++)
        {
            Vector3 dropPos = BomGridRules.GetCellInDirection(originCell, direction, distance);

            if (false == CanDropBom(dropPos))
            {
                break;
            }
            // 通常の爆弾投下と以下処理は共通化出来る。
            BomParameters bomParams = cPlayerBom.CreateBomParameters(dropPos, direction);
            GameObject cBom = cBomControl.DropBom(bomParams);
            cPlayerBom.Add(cBom);
        }
    }

    private void RequestExplodeThrow(){
        /*
         ExplodeAttackManagerを作成して依頼する
         爆風の数を渡すようにして、爆風の数分アタックできる仕様とする。
        */
    }

    public void RequestDropBom(){
        GetAction().Invoke();
    }

    protected bool CanDropBom(Vector3 position){
        if(false == cGameManager.GetSetUp()){
            return false;
        }
		if(Library_Base.IsPositionOutOfBounds(position)){
			return false;
		}
        if(false == cPlayerBom.IsBomAvailable(position)){
            return false;
        }
        // 所有者が異なるボムも含め、同じグリッドへは設置しない。
        if(BomGridRules.IsBombAtCell(position)){
            return false;
        }
        if(cField != null && cField.IsBlockedForBomb(position)){
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

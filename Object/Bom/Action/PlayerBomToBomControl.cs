using System;
using UnityEngine;
public class PlayerBomToBomControl : MonoBehaviour
{
    protected BomControl cBomControl;
    protected PlayerBom cPlayerBom;

    GameManager cGameManager;

    ItemControl cItemControl;

    public void Awake(){
        cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
        cPlayerBom = this.gameObject.AddComponent<PlayerBom>();
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cItemControl = GameObject.Find("ItemControl").GetComponent<ItemControl>();
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
        BomParameters bomParams = cPlayerBom.CreateBomParameters(position, transform.forward);
        GameObject cBom = cBomControl.DropBom(bomParams);
        cPlayerBom.Add(cBom);
    }
    private void RequestDropBomMulti(){
        Vector3 currentPos = transform.position;
        Vector3 direction = transform.forward;

        while (true)
        {
            // 方向に1マス進める
            currentPos += direction;
            Vector3 dropPos = Library_Base.GetPos(currentPos);

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
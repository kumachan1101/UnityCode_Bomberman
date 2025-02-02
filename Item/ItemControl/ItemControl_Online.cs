using UnityEngine;
using Photon.Pun;
public class ItemControl_Online: ItemControl
{
    protected override void CreateItem_AddList(){
        // アイテムPrefabのロード
        GameObject ItemFirePrefab = Resources.Load<GameObject>("item_fire");
        GameObject ItemBomPrefab = Resources.Load<GameObject>("item_bom");
        GameObject ItemBomExplodePrefab = Resources.Load<GameObject>("item_explode");
        GameObject ItemBomBigBanPrefab = Resources.Load<GameObject>("item_bigban");
        GameObject ItemSpeedPrefab = Resources.Load<GameObject>("item_speedup");
        GameObject ItemBomkickPrefab = Resources.Load<GameObject>("item_bomkick");
        GameObject ItemBomAttackPrefab = Resources.Load<GameObject>("item_bomattack");
        GameObject ItemRainbowPrefab = Resources.Load<GameObject>("item_rainbow");
        GameObject ItemHeartPrefab = Resources.Load<GameObject>("item_heart");
        GameObject ItemAddBlockPrefab = Resources.Load<GameObject>("item_addblock");
        GameObject ItemAddDummyPrefab = Resources.Load<GameObject>("item_adddummy");

        // アイテムリストに追加
        itemList.Add(new CreateItem { itemName = "Item_FireUp", itemPrefab = ItemFirePrefab });
        itemList.Add(new CreateItem { itemName = "Item_BomUp", itemPrefab = ItemBomPrefab });
        itemList.Add(new CreateItem { itemName = "Item_BomExplode", itemPrefab = ItemBomExplodePrefab });
        itemList.Add(new CreateItem { itemName = "Item_BomBigBan", itemPrefab = ItemBomBigBanPrefab });
        itemList.Add(new CreateItem { itemName = "Item_SpeedUp", itemPrefab = ItemSpeedPrefab });
        itemList.Add(new CreateItem { itemName = "Item_BomKick", itemPrefab = ItemBomkickPrefab });
        itemList.Add(new CreateItem { itemName = "Item_BomAttack", itemPrefab = ItemBomAttackPrefab });
        itemList.Add(new CreateItem { itemName = "Item_Rainbow", itemPrefab = ItemRainbowPrefab });
        itemList.Add(new CreateItem { itemName = "Item_Heart", itemPrefab = ItemHeartPrefab });
        itemList.Add(new CreateItem { itemName = "Item_AddBlock", itemPrefab = ItemAddBlockPrefab });
        itemList.Add(new CreateItem { itemName = "Item_AddDummy", itemPrefab = ItemAddDummyPrefab });
    }

    public override void CreateItem_RPC(Vector3 v3){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        photonView.RPC(nameof(CreateRandomItem), RpcTarget.All, v3);
    }

    public override GameObject CreateRandomItem(Vector3 position)
    {
        // アイテム生成の確率
        if (Random.value <= 0.2f)  // 20%の確率でアイテムを生成
        {
            int randomIndex = Random.Range(0, itemList.Count);
            CreateItem selectedItem = itemList[randomIndex];

            // アイテムをインスタンス化
            GameObject itemInstance = Instantiate(selectedItem.itemPrefab, position, Quaternion.identity);
            //Debug.Log("Created item: " + selectedItem.itemName);
            return itemInstance;
        }
        else
        {
            //Debug.Log("No item spawned this time.");
            return null;
        }
    }


}

using UnityEngine;

public class ItemControl_TowerMode: ItemControl_CpuMode
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
        itemList.Add(new CreateItem { itemName = "Item_AddBlock", itemPrefab = ItemAddBlockPrefab });
        for(int i = 0; i < 10; i++) {
            itemList.Add(new CreateItem { itemName = "Item_AddDummy", itemPrefab = ItemAddDummyPrefab });
        }

    }

}

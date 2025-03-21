using System.Collections.Generic;
using UnityEngine;

public class ItemPathProvider_CpuMode : IItemPathProvider
{
    public Dictionary<string, string> GetItemPaths() => new Dictionary<string, string>
    {
        { "Item_FireUp", "item_fire" },
        { "Item_BomUp", "item_bom" },
        { "Item_BomExplode", "item_explode" },
        { "Item_BomBigBan", "item_bigban" },
        { "Item_SpeedUp", "item_speedup" },
        { "Item_BomKick", "item_bomkick" },
        { "Item_BomAttack", "item_bomattack" },
        { "Item_Rainbow", "item_rainbow" },
        { "Item_Heart", "item_heart" },
        { "Item_AddBlock", "item_addblock" }
        //{ "Item_AddDummy", "item_adddummy" }
    };
}

public class ItemControl_CpuMode: ItemControl
{
    protected override IItemPathProvider CreateItemPathProvider(){
        return new ItemPathProvider_CpuMode();
    }
    public override void CreateItem_RPC(Vector3 v3){
        if(false == IsCreateItem()){
            return;
        }    
        int randomIndex = Random.Range(0, itemList.Count);
        CreateRandomItem(v3, randomIndex);
    }

    protected override bool IsCreateItem()
    {
        // アイテム生成の確率
        if (Random.value <= 1f)  
        {
            return true;
        }
        return false;
    }


}

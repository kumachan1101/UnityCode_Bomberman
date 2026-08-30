using UnityEngine;
using System.Collections.Generic;

public class ItemPathProvider_Tower : IItemPathProvider, IItemCountProvider
{
    public Dictionary<string, string> GetItemPaths() => new Dictionary<string, string>
    {
        { "Item_FireUp", "item_fire" },
        { "Item_BomUp", "item_bom" },
        { "Item_BomExplode", "item_explode" },
        { "Item_BomBigBan", "item_bigban" },
        { "Item_SpeedUp", "item_speedup" },
        { "Item_BomSpeedUp", "item_bomspeedup" },
        { "Item_BomKick", "item_bomkick" },
        { "Item_BomAttack", "item_bomattack" },
        { "Item_BomMulti", "item_bommulti" },
        //{ "Item_Rainbow", "item_rainbow" },
        { "Item_Heart", "item_heart" },
        { "Item_AddBlock", "item_addblock" },
        { "Item_AddDummy", "item_adddummy" },
        { "Item_Invincible", "item_invincible" },
        { "Item_Magnet", "item_invincible" }
    };

    public Dictionary<string, int> GetItemCounts() => new Dictionary<string, int>
    {
        { "Item_AddDummy", 3 } // 特殊なループ処理
    };
}

public class ItemControl_TowerMode: ItemControl_CpuMode
{
    protected override IItemPathProvider CreateItemPathProvider(){
        return new ItemPathProvider_Tower();
    }

}

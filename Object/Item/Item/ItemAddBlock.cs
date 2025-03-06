
using UnityEngine;
public class ItemAddBlock : Item{
    public override void Reflection(GameObject gObj){
        BlockCreateManager cField = GameObject.Find("Field").GetComponent<BlockCreateManager>();
        cField.AddBrokenBlock(20);
    }
}

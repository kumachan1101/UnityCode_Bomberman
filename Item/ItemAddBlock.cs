
using UnityEngine;
public class ItemAddBlock : Item{
    public override void Reflection(string objname){
        Field_Block_Base cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
        cField.AddBrokenBlock(20);
    }
}


using UnityEngine;
public class ItemAddBlock : Item{
    public override void Reflection(GameObject gObj){
        Field_Block_Base cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
        cField.AddBrokenBlock(20);
    }
}

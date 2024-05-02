using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemAddBlock : Item{
    public override void Reflection(string objname){
        Field cField = GameObject.Find("Field").GetComponent<Field>();
        cField.AddBrokenBlock();
    }
}

using UnityEngine;
public class ItemBomSpeedUp : Item{
    public override void Reflection(GameObject gObj){
        PlayerMovement cPlayerAction = gObj.GetComponent<PlayerMovement>();
        if(null != cPlayerAction){
            //cPlayerAction.SpeedUp();
        }
    }
}

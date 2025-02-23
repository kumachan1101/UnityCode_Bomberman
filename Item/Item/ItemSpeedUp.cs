using UnityEngine;
public class ItemSpeedUp : Item{
    public override void Reflection(GameObject gObj){
        PlayerMovement cPlayerAction = gObj.GetComponent<PlayerMovement>();
        if(null != cPlayerAction){
            cPlayerAction.SpeedUp();
        }
    }
}

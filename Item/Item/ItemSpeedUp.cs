using UnityEngine;
public class ItemSpeedUp : Item{
    public override void Reflection(GameObject gObj){
        PlayerAction cPlayerAction = gObj.GetComponent<PlayerAction>();
        if(null != cPlayerAction){
            cPlayerAction.SpeedUp();
        }
    }
}

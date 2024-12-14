using UnityEngine;
public class ItemHeartUp : Item{
    public override void Reflection(GameObject gObj){
        PowerGageIF cPowerGageIF = gObj.GetComponent<PowerGageIF>();
        cPowerGageIF.HeartUp(5);
    }
}

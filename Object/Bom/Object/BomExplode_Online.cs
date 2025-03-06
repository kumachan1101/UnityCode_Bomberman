using UnityEngine;

public class BomExplode_Online : Bom_Online
{
	
    // IsBroken 判定を行わないようにする
    protected override bool ShouldCheckIsBroken()
    {
        return false;
    }

}

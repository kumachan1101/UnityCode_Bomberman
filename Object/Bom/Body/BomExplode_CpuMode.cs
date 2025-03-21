using UnityEngine;

public class BomExplode_CpuMode : Bom_CpuMode
{
	
    // IsBroken 判定を行わないようにする
    protected override bool ShouldCheckIsBroken()
    {
        return false;
    }

}

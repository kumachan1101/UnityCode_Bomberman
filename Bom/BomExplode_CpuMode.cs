using UnityEngine;

public class BomExplode_CpuMode : Bom_CpuMpde
{
	
    // IsBroken 判定を行わないようにする
    protected override bool ShouldCheckIsBroken()
    {
        return false;
    }

}

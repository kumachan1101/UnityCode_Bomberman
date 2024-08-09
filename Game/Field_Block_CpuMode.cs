using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
public class Field_Block_CpuMode : Field_Block_Base {
    private GameManager cGameManager;




    protected override void ClearBrokenList_RPC(){
        ClearBrokenList();
    }


    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        InsBrokenBlock(x, y, z);
    }


    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
        InsObjMove(x, y, z, randomDirection);
    }


    protected override void Rainbow_RPC(string sMaterialType){
        Rainbow(sMaterialType);
    }

	protected override void InsPoolExplosion_RPC(){
		//string sMaterialType = GetComponent<Field_Player_Base>().GetExplosionMaterialType(GetComponent<Field_Player_Base>().GetIndex());
	}

    public override string GetExplosionType(string input)
    {
        if (input.Contains(ExplosionTypes.Explosion1))
        {
            return ExplosionTypes.Explosion1;
        }
        else if (input.Contains(ExplosionTypes.Explosion2))
        {
            return ExplosionTypes.Explosion2;
        }
        else if (input.Contains(ExplosionTypes.Explosion3))
        {
            return ExplosionTypes.Explosion3;
        }
        else if (input.Contains(ExplosionTypes.Explosion4))
        {
            return ExplosionTypes.Explosion4;
        }
        else
        {
            return null;
        }
    }



}
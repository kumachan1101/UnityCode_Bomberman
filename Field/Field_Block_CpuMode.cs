
public class Field_Block_CpuMode : Field_Block_Base {

    protected override void ClearBrokenList_RPC(){
        ClearBrokenList();
    }


    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        InsBrokenBlock(x, y, z);
    }


    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
        InsObjMove(x, y, z, randomDirection);
    }


    public override void Rainbow_RPC(string sMaterialType){
        Rainbow(sMaterialType);
    }

}
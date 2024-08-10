using UnityEngine;
public class Player_CpuMode : Player_Base
{

    public override void UpdateKey(){
        DropBom();
    }

    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction_CpuMode(ref rigidBody, ref myTransform);
    }


    protected override bool IsAvairable(){
        if(iViewID == -1){
            return false;
        }
        return true;
    }

	protected override void DestroySync(GameObject g){
		Destroy(g);
	}

}

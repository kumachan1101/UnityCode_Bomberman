using UnityEngine;
public class Player : Player_Base
{
	void Awake() {
		//基底クラスで以下を関数化して、基底クラスの関数をコールするのが良い。
		cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();	
		cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
	}
/*
    public override void SetSlider(GameObject gCanvas){
        cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
    }
*/
    public override void UpdateKey(){
        if (Input.GetKey(KeyCode.Return)) {
             if (pushFlag == false){
                pushFlag = true;
                DropBom();
                //AttackExplosion();
             }
        }
        else{
            pushFlag = false;
        }
    }

    protected override bool IsAvairable(){
        if(iViewID == -1){
            return false;
        }
        return true;
    }


    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction(ref rigidBody, ref myTransform);
    }
	protected override void DestroySync(GameObject g){
		Destroy(g);
	}

}

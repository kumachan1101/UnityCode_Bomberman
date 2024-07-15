using UnityEngine;
public class Player_CpuMode : Player_Base
{

	void Awake() {
		cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();	
		cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
	}

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

/*
    public override void SetSlider(GameObject gCanvas){
		Debug.Log("SetSlider");
        // ベースクラスを取得するすれば、派生クラスのスクリプトは取得可能であるため、1か所にまとめること
        cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage_CpuMode>();
		
    }
*/

}

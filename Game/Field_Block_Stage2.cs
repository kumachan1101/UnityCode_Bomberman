using UnityEngine;
using UnityEngine.UI;
public class Field_Block_Stage2 : Field_Block_CpuMode {
	protected override void SetFieldRange(){
		GameManager.SetFieldRange(15,15);
	}

}
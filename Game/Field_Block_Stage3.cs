using UnityEngine;
using UnityEngine.UI;
public class Field_Block_Stage3 : Field_Block_CpuMode {
	protected override void SetFieldRange(){
		GameManager.SetFieldRange(20,20);
	}

}
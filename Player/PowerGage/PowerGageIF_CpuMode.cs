using UnityEngine;
public class PowerGageIF_CpuMode : PowerGageIF
{
	void Start(){
		if(cPowerGage == null){
			GameObject gCanvas = Library_Base.FindGameObjectByInstanceID(iCanvasInsID);
			if(gCanvas != null){
				GameObject sliderObject = gCanvas.transform.Find("Slider").gameObject;
				cPowerGage = sliderObject.AddComponent<PowerGage_CpuMode>();
			}
		}
	}
}



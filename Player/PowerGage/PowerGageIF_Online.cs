using UnityEngine;
using Photon.Pun;
public class PowerGageIF_Online : PowerGageIF
{
	void Start(){
		if(cPowerGage == null){
			PhotonView viewPlayer = PhotonView.Find(iCanvasInsID);
			if (viewPlayer == null)
			{
				Debug.LogError("ViewFind Error : "+iCanvasInsID);
				return;
			}

			GameObject gCanvas = viewPlayer.gameObject;
			if (gCanvas == null)
			{
				Debug.LogError("GameObject Error");
				return;
			}

			GameObject sliderObject = gCanvas.transform.Find("Slider").gameObject;
			cPowerGage = sliderObject.AddComponent<PowerGage_Online>();
		}
	}
}



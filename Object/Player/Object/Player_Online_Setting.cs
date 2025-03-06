using System;
using Photon.Pun;
using UnityEngine;

public class Player_Online_Setting : MonoBehaviour, IPunInstantiateMagicCallback
{
	private int iCanvasInsID;
	private int iPlayerNo;

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		object[] data = info.photonView.InstantiationData;
		iCanvasInsID = (int)data[0];
		iPlayerNo = (int)data[1];
		string scriptName = (string)data[2];
		Player_Base cPlayer = null;
		if (!string.IsNullOrEmpty(scriptName))
		{
			// 指定されたスクリプト名から Type を取得
			Type scriptType = Type.GetType(scriptName);
			if (scriptType != null)
			{
				// AddComponent を動的に実行
				cPlayer = (Player_Base)this.gameObject.AddComponent(scriptType);
			}
			else
			{
				Debug.LogError($"スクリプト {scriptName} が見つかりませんでした。");
			}
		}

		this.gameObject.name = "Player" + iPlayerNo;

		// PowerGageクラスがネットワーク上全てに反映されてない。もしかしたら、CanvasIDはローカルだけかも。PothonViewIDで同期した方が良いのかも。
		PowerGageIF cPowerGageIF = this.gameObject.AddComponent<PowerGageIF_Online>();
		cPowerGageIF.SetCanvasInsID(iCanvasInsID);

		cPlayer.AddPlayerComponent();
	}

	void Start()
	{
		
	}
}

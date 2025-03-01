using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
abstract public class PowerGageIF : MonoBehaviourPunCallbacks
{
	protected PowerGage cPowerGage;
	protected int iCanvasInsID;
	protected GameObject gCanvas;
	void Start(){
		if(cPowerGage == null){
			gCanvas = Library_Base.FindGameObjectByInstanceID(iCanvasInsID);
			if(gCanvas != null){
				GameObject sliderObject = gCanvas.transform.Find("Slider").gameObject;
				cPowerGage = CreatePowerGage(sliderObject);
			}
		}
	}
    protected virtual PowerGage CreatePowerGage(GameObject sliderObject)
    {
        return sliderObject.AddComponent<PowerGage>();
    }


	public void SetPowerGage(PowerGage cObj){
		cPowerGage = cObj;
	}

	public void SetCanvasInsID(int iID){
		//Debug.Log(iID);
		iCanvasInsID = iID;
	}

    public void SetDamage(int iDamage){
		//Debug.Log(cPowerGage);
        if(cPowerGage == null){
            return;
        }
        SetDamage_RPC(iDamage);
	}

	protected virtual void SetDamage_RPC(int iDamage){}

	protected virtual Component GetDestroyTarget() => GetComponent<Player_Base>();

	[PunRPC]
	public virtual void SyncSetDamage(int iDamage)
	{
		if (cPowerGage == null)
		{
			Debug.Log("cPowerGage is null");
			StartCoroutine(RetrySyncSetDamage(iDamage));
			return;
		}
		cPowerGage.SetDamage(iDamage);
		if (cPowerGage.IsDead())
		{
			GetDestroyTarget()?.SendMessage("DestroySync");
			DestroySync();
		}
	}


	protected void DestroySync(){
		//Debug.Log(gCanvas);
		Destroy(gCanvas);
	}
	protected IEnumerator RetrySyncSetDamage(int iDamage){
		int retries = 5; // 再試行回数
		float delay = 0.1f; // 再試行間隔
		while (retries > 0){
			if (cPowerGage != null) {
				SyncSetDamage(iDamage); // cPowerGage が初期化済みなら SyncSetDamage を呼び出す
				yield break;
			}
			retries--;
			yield return new WaitForSeconds(delay);
		}
		Debug.LogError("Failed to initialize cPowerGage after retries.");
	}

	public void HeartUp(int iHeart){
        if(cPowerGage == null){
            return;
        }
		HeartUp_RPC(iHeart);
	}
	protected virtual void HeartUp_RPC(int iHeart){}
	[PunRPC]
	public void SyncHeartUp(int iHeart){
        if(cPowerGage == null){
			Debug.Log("cPowerGage is null");
            return;
        }
		cPowerGage.HeartUp(iHeart);
	}

}



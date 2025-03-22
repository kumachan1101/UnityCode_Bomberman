using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

abstract public class PowerGageIF : MonoBehaviourPunCallbacks
{
	protected PowerGage cPowerGage;
	protected int iCanvasInsID;
	protected GameObject gCanvas;
    public static GameObjectEvent onPowerGageAdded = new GameObjectEvent();
    public static GameObjectEvent onPowerGageRemoved = new GameObjectEvent();

	void Start(){
		if(cPowerGage == null){
			gCanvas = Library_Base.FindGameObjectByInstanceID(iCanvasInsID);
			if(gCanvas != null){
				//GameObject sliderObject = gCanvas.transform.Find("Slider").gameObject;
				//cPowerGage = CreatePowerGage(sliderObject);
				cPowerGage = CreatePowerGage(gCanvas);

				onPowerGageAdded.Invoke(gCanvas);  // 自分が追加されたことを通知
			}
		}
	}
    void OnDestroy()
    {
    }

    protected virtual PowerGage CreatePowerGage(GameObject sliderObject)
    {
        return sliderObject.AddComponent<PowerGage>();
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
			//StartCoroutine(RetrySyncSetDamage(iDamage));
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
		onPowerGageRemoved.Invoke(gCanvas);  // 自分が削除されたことを通知
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



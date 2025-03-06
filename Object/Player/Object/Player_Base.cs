using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
public class Player_Base : MonoBehaviourPunCallbacks
{
    // プレイヤーの生成・削除イベント
    public static UnityEvent<Player_Base> onPlayerAdded = new UnityEvent<Player_Base>();
    public static UnityEvent<Player_Base> onPlayerRemoved = new UnityEvent<Player_Base>();

    void Awake(){
   }    
    void Start ()
    {
        onPlayerAdded.Invoke(this);  // 自分が追加されたことを通知
	}
    public virtual void AddPlayerComponent(){}

	public virtual void DestroySync(){
		Destroy(this.gameObject);
	}

    void OnDestroy()
    {
        onPlayerRemoved.Invoke(this);  // 自分が削除されたことを通知
    }
}
using UnityEngine;
using Photon.Pun;
public class Player_Base : MonoBehaviourPunCallbacks
{
    void Awake(){
        //this.gameObject.AddComponent<PowerGageIF>();
    }    
    void Start ()
    {
		//InitComponent();
        //AddPlayerComponent();//ローカルとオンラインでスクリプトをAddしないといけない。つまりRPC同期が必要。左記以外は、最初から追加しておく。もしくはPothhonのインスタンス生成時にAddsするか
	}
/*
	protected void InitComponent(){
        this.gameObject.AddComponent<Player_Collision>();
        this.gameObject.AddComponent<Player_Texture>();
        AddPlayerComponent();
	}
*/
    public virtual void AddPlayerComponent(){}

	public virtual void DestroySync(){
		Destroy(this.gameObject);
	}

}
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Explosion_NotGround : Explosion_Base
{
    protected override void hide(){
        /*
		if(null == cExplosionManager){
			cExplosionManager = GameObject.Find("ExplosionManager").GetComponent<ExplosionManager>();
		}
        */
        // アクティブ判定しないと、爆風を暫く表示し続けると、FALSEの爆風が生成され始めて、表示されなくなる。調査必要
    	//if (gameObject.activeInHierarchy){
        if (gameObject.activeSelf){
            cExplosionManager.EnqueueObject(this.gameObject);
        }
        else{
            Debug.Log("activeSelf false");
        }
    }

}

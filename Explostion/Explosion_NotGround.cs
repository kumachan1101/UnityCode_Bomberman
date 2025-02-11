using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Explosion_NotGround : Explosion_Base
{
    protected override void hide(){
		if(null == cExplosionManager){
			cExplosionManager = GameObject.Find("ExplosionManager").GetComponent<ExplosionManager>();
		}
        cExplosionManager.EnqueueObject(this.gameObject);
    }

}

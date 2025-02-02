using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Explosion_NotGround : Explosion_Base
{
    protected override void hide(){
		if(null == cField){
			cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
		}
        cField.EnqueueObject(this.gameObject);
    }

}

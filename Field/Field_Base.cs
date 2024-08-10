using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Field_Base :MonoBehaviourPunCallbacks {
    protected virtual void Init(){}
    protected virtual void GameTransision(){}
	//protected virtual void DestroySync(GameObject g){}

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        GameTransision();        
		
    }

}
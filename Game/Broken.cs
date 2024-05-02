using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broken : MonoBehaviour
{
    protected GameObject gItemControl;
    protected ItemControl cItemControl;

    void Awake(){
        cItemControl = GameObject.Find("ItemControl").GetComponent<ItemControl>();
        //g.transform.position = transform.position;
        /*
        if(null == gItemControl){
            gItemControl = GameObject.Find("ItemControl(Clone)");
            if(null != gItemControl){
                cItemControl = gItemControl.GetComponent<ItemControl>();
            }
        }
        */

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision col) {
        //Debug.Log($"{col.transform.name} is OnCollisionEnter.");
        if("Explosion(Clone)" == col.transform.name){
            DelBox();
        }
        /*
        else if("Player1(Clone)" == col.transform.name){
            Debug.Log($"{col.transform.name} is OnCollisionEnter.");
            GetComponent<Collider>().isTrigger = true;
        }
        */

    }
    private void OnTriggerEnter(Collider col){
        //Debug.Log($"{col.transform.name} is OnTriggerEnter");
        if("Explosion(Clone)" == col.transform.name){
            DelBox();
        }
    }

    private void DelBox(){
        cItemControl.CreateRandItem(transform.position);
        Destroy(gameObject);
    }

    public void SetIsTrigger(bool bSet){
        GetComponent<Collider>().isTrigger = bSet;        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BomName;
public class Explosion : MonoBehaviour
{
    private Field cField;
    private bool bField = false;

    public void FieldValid()
    {
        bField = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(bField){
            //cField.AddExplosion(this.gameObject);
            return;
        }
        Invoke(nameof(hide), 1f);
    }

    void hide(){
        transform.position = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
        cField = GameObject.Find("Field").GetComponent<Field>();        
        cField.UpdateGroundExplosion(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetDamage(){
        return 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.transform.name)
        {
            case "Bom(Clone)":
            case "Bombigban(Clone)":
            case "BomExplode(Clone)":
                GameObject collidedObject = other.gameObject;
                collidedObject.GetComponent<Bom>().CancelInvokeAndCallExplosion();
                break;
            default:
                return;
        }
    }
}

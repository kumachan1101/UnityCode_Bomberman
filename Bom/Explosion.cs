using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BomName;
public class Explosion : MonoBehaviour
{
    private Field cField;
    protected bool bField = false;

    //private bool bStopInvoke = false;

    private SoundManager soundManager;

    public void FieldValid()
    {
        bField = true;
    }

    void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(bField){
            //cField.AddExplosion(this.gameObject);
            return;
        }

        Invoke(nameof(hide), 1f);

        
        soundManager.PlaySoundEffect("EXPLOISON");
    }

/*
    public void StopInvoke(){
        CancelInvoke(nameof(hide));
        bStopInvoke = true;
    }
*/
    void hide(){
        transform.position = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
        cField = GameObject.Find("Field").GetComponent<Field>();
        bool bRet = cField.IsMatchObjMove(transform.position);
        if(bRet){
            Destroy(gameObject);
        }
        else{
            cField.UpdateGroundExplosion(this.gameObject);
        }
        
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
            case "FixedWall(Clone)":
                Destroy(gameObject);
                //transform.position = GetPos();
                //hide();
                break;
            default:
                return;
        }
    }

    private Vector3 GetPos(){
        float x = Mathf.Round(transform.position.x);
        float y = 1;
        float z = Mathf.Round(transform.position.z);
        return new Vector3(x,y,z);
    }

}

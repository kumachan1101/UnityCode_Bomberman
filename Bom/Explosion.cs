using UnityEngine;
using Photon.Pun;
public class Explosion : MonoBehaviourPunCallbacks
{
    private Field_Base cField;
    protected bool bField = false;
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
    void hide(){
        
        transform.position = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
        cField = GameObject.Find("Field").GetComponent<Field_Base>();
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
                collidedObject.GetComponent<Bom_Base>().CancelInvokeAndCallExplosion();
                break;
            case "FixedWall(Clone)":
                Destroy(gameObject);
                break;
            default:
                return;
        }
    }

}

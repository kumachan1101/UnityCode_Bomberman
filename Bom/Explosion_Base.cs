using UnityEngine;
using Photon.Pun;
public class Explosion_Base : MonoBehaviourPunCallbacks
{
    private Field_Block_Base cField;
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
        cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
        bool bRet = cField.IsMatchObjMove(transform.position);
        if(bRet){
            DestroySync(gameObject);
        }
        else{
            cField.UpdateGroundExplosion(this.gameObject);
        }
        
    }

	protected virtual void DestroySync(GameObject g){}

	protected virtual bool IsSync(){
		return false;
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
        if(false == IsSync()){
            return;
        }
        switch (other.transform.name)
        {
            case "FixedWall(Clone)":
                DestroySync(gameObject);
                break;
            default:
                return;
        }
    }

}

using UnityEngine;
using Photon.Pun;

public class Explosion_Base : MonoBehaviour
{
    protected Field_Block_Base cField;
    protected bool bField = false;
    private SoundManager soundManager;

    int iID;

    public void SetID(int id){
        iID = id;
    }

    public int GetID(){
        return iID;
    }

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
		cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
        //Invoke(nameof(hide), 1f);
        soundManager.PlaySoundEffect("EXPLOISON");
        if (Library_Base.IsPositionOutOfBounds(transform.position)){
            DestroySync(this.gameObject);
        }

    }

    void update(){
        if (Library_Base.IsPositionOutOfBounds(transform.position)){
            DestroySync(this.gameObject);
        }

    }

	public void ReqHide(){
		Invoke(nameof(hide), 1f);
	}

    void hide(){
        SetPosition(new Vector3(transform.position.x, transform.position.y-1, transform.position.z));
		if(null == cField){
			cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
		}
        bool bRet = cField.IsMatchObjMove(transform.position);
        if(bRet){
            DestroySync(this.gameObject);
        }
        else{
			if (gameObject.activeInHierarchy){
				cField.UpdateGroundExplosion(this.gameObject);
			}
        }
    }

	protected virtual void DestroySync(GameObject g){
		if (g == null)
		{
			Debug.LogWarning("Instance is null, cannot enqueue.");
			return;
		}
		if(null == cField){
            //Debug.Log(g.transform.position);
			cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
		}
		cField.EnqueueObject(g);
	}



	protected bool IsSync(){
		return true;
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
                //Debug.Log(gameObject.transform.position);
                DestroySync(gameObject);
                break;
            default:
                return;
        }
    }


	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}

}

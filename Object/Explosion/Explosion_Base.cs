using UnityEngine;

public class Explosion_Base : MonoBehaviour
{
    protected ExplosionManager cExplosionManager;
    protected BlockCreateManager cField;
    protected bool bField = false;
    private SoundManager soundManager;

    void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        if(bField){
            //cField.AddExplosion(this.gameObject);
            return;
        }
        */
		cExplosionManager = GameObject.Find("ExplosionManager").GetComponent<ExplosionManager>();
        cField = GameObject.Find("Field").GetComponent<BlockCreateManager>();
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

    protected virtual void hide(){
		if(null == cExplosionManager){
			cExplosionManager = GameObject.Find("ExplosionManager").GetComponent<ExplosionManager>();
		}
        if(null == cField){
            cField = GameObject.Find("Field").GetComponent<BlockCreateManager>();
        }
        SetPosition(new Vector3(transform.position.x, transform.position.y-1, transform.position.z));
        bool bRet = cField.IsMatchObjMove(transform.position);
        if(bRet){
            DestroySync(this.gameObject);
        }
        else{
			if (gameObject.activeInHierarchy){
				cExplosionManager.UpdateGroundExplosion(this.gameObject);
			}
        }
    }

	protected virtual void DestroySync(GameObject g){
		if (g == null)
		{
			Debug.LogWarning("Instance is null, cannot enqueue.");
			return;
		}
		if(null == cExplosionManager){
            //Debug.Log(g.transform.position);
			cExplosionManager = GameObject.Find("ExplosionManager").GetComponent<ExplosionManager>();
		}
		cExplosionManager.EnqueueObject(g);
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

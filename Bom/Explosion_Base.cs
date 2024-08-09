using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
//[RequireComponent(typeof(PhotonTransformView))]
public class Explosion_Base : MonoBehaviourPunCallbacks
{
    protected Field_Block_Base cField;
    protected bool bField = false;
    private SoundManager soundManager;
    private PhotonTransformView transformView;
    public void FieldValid()
    {
        bField = true;
    }

    void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        // PhotonTransformView コンポーネントを取得
		/*
        transformView = GetComponent<PhotonTransformView>();

        if (transformView != null)
        {
        }
		*/
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
    }

	public void ReqHide(){
		Invoke(nameof(hide), 1f);
	}

    void hide(){
        SetPosition_RPC(new Vector3(transform.position.x, transform.position.y-1, transform.position.z));
        //transform.position = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
		if(null == cField){
			cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
		}
        bool bRet = cField.IsMatchObjMove(transform.position);
        if(bRet){
            DestroySync(gameObject);
        }
        else{
			if (gameObject.activeInHierarchy){
				cField.UpdateGroundExplosion(this.gameObject);
			}
			else{
				Debug.Log("hide no active");
			}
            
        }
        
    }

	protected virtual void DestroySync(GameObject g){
		if (g == null)
		{
			Debug.LogWarning("Instance is null, cannot enqueue.");
			return;
		}
		//Field_Block_Base cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
		cField.EnqueueObject(g);

		//Destroy(g);
	}



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

	public virtual void SetPosition_RPC(Vector3 position){}

	[PunRPC]
	public void SetPosition(Vector3 position)
	{
		//Debug.Log("SetPosition called with position: " + position);
		transform.position = position;
	}

}

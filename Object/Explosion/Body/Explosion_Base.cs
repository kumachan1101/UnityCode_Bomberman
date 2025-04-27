using UnityEngine;

public class Explosion_Base : MonoBehaviour
{
    protected ExplosionManager cExplosionManager;
    protected BlockCreateManager cField;
    protected bool bField = false;
    private SoundManager soundManager;

    void update(){
        if (Library_Base.IsPositionOutOfBounds(transform.position)){
            DestroySync(this.gameObject);
        }

    }

    private bool Setup(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		cExplosionManager = GameObject.Find("ExplosionManager").GetComponent<ExplosionManager>();
        cField = GameObject.Find("Field").GetComponent<BlockCreateManager>();
        soundManager.PlaySoundEffect("EXPLOISON");
        if (Library_Base.IsPositionOutOfBounds(transform.position)){
            DestroySync(this.gameObject);
            return false;
        }
        return true;
    }

	public void ReqActive(){
        if(false == Setup()){
            return;
        }
		Invoke(nameof(hide), 1f);
	}

    public void ReqCancel () {
        CancelInvoke(nameof(hide));
    }

    protected virtual void hide(){
        /*
		if(null == cExplosionManager){
			cExplosionManager = GameObject.Find("ExplosionManager").GetComponent<ExplosionManager>();
		}
        if(null == cField){
            cField = GameObject.Find("Field").GetComponent<BlockCreateManager>();
        }
        */
        SetPosition(new Vector3(transform.position.x, transform.position.y-1, transform.position.z));
        bool bRet = cField.IsMatchObjMove(transform.position);
        if(bRet){
            DestroySync(this.gameObject);
        }
        else{
            /*
                爆風の重なりで同一色と判断された場合など、Hideがコールされる前に、エンキューされてしまう事がある。
                エンキューする際に、Hideのキャンセルを実行しているが、非同期で動作することもあり、Hideが発動された後に
                エンキュー処理が動き、Hideのキャンセルをしてももう間に合わず、その場合は、アクティブかどうか判定することで、
                非アクティブになってしまった後でも、処理しないで済む。                
            */

            if (gameObject.activeSelf){
				cExplosionManager.UpdateGroundExplosion(this.gameObject);
			}
            else{
                Debug.Log("activeSelf false");
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

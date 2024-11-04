using UnityEngine;
using Photon.Pun;

public class Player_Base : MonoBehaviour
{

    protected string MaterialType;
    protected Rigidbody rigidBody;
    protected Transform myTransform;
    protected Animator animator;
    //protected Field_Base cField;
    protected bool pushFlag = false;
    public int iViewID = -1;
    protected PowerGage cPowerGage;
    protected PlayerBom cPlayerBom;
    [SerializeField]protected PlayerAction cPlayerAction;
    protected Library_Base cLibrary;

	protected BomControl cBomControl;

	protected JoystickController cJoystickController;

    void Awake ()
    {
		InitComponent();

		GameObject gField = GameObject.Find("Field");
		Field_Player_Base fieldPlayerBase = gField.GetComponent<Field_Player_Base>();
		//fieldPlayerBase.AddPlayerCnt(); 
		SetPlayerSetting(fieldPlayerBase.GetPlayerCnt());

	}

	protected void InitComponent(){
		cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();	
		cBomControl = GameObject.Find("BomControl").GetComponent<BomControl>();
	}

    public void SetSlider(GameObject gCanvas){
		cPowerGage = gCanvas.transform.Find("Slider").GetComponent<PowerGage>();
		//Debug.Log(cPowerGage);
		if(cPowerGage == null){
			//Debug.Log("cPowerGage is null");
		}
	}

    protected virtual bool IsAvairable(){
        return false;
    }

    public virtual void UpdateKey(){}

    protected virtual void CreatePlayerAction(){}

	public virtual void SetPlayerSetting(int iParamViewID)
	{
		//Debug.Log(iParamViewID);
		SetViewID(iParamViewID);
		InitializeMaterialType();
		InitializePlayerBom();
		InitializeRigidbody();
		InitializeTransform();
		InitializeAnimator();
		//InitializeField();
		InitializePlayerAction();
		FindJoystickPlayer();
	}

	protected void SetViewID(int viewID)
	{
		iViewID = viewID;
	}

	protected void InitializeMaterialType()
	{
		MaterialManager materialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
		MaterialType = materialManager.GetBomMaterialByPlayerName(this.gameObject.name);
	}

	protected void InitializePlayerBom()
	{
		CreatePlayerBom();
		cPlayerBom.SetMaterialType(MaterialType);
	}

	protected void InitializeRigidbody()
	{
		rigidBody = GetComponent<Rigidbody>();
	}

	protected void InitializeTransform()
	{
		myTransform = transform;
	}

	protected void InitializeAnimator()
	{
		animator = GetComponent<Animator>();
	}

	protected void InitializeField()
	{
		//cField = GetField();
	}

	protected void InitializePlayerAction()
	{
		CreatePlayerAction();
		cPlayerAction.SetMaterialType(MaterialType);
	}

	protected void FindJoystickPlayer()
	{
		GameObject joystickPlayer = GameObject.Find("JoystickPlayer");
		cJoystickController = joystickPlayer.GetComponent<JoystickController>();
	}


    // Update is called once per frame
    void Update ()
    {
        if(false == IsAvairable() || null == cPlayerAction){
            return;
        }
        UpdatePlayer();
    }

    protected void UpdatePlayer(){
        cPlayerAction.UpdateMovement();
        UpdateKey();
        UpdateJoyStick();
    }

	private void UpdateJoyStick(){
		Vector3 v3 = cJoystickController.GetMoveVector();
        if (v3 == Vector3.zero)
        {
            return;
        }
		cPlayerAction.Move(v3);
	}

    public void DropBom(){
		
        if(null == cLibrary || null == cBomControl){
			return;
        }
		if(Library_Base.IsPositionOutOfBounds(transform.position)){
			return;
		}
        Vector3 v3 = Library_Base.GetPos(transform.position);
        if(false == cPlayerBom.IsBomAvailable(v3)){
            return;
        }
        Vector3 direction = myTransform.forward;
        BomParameters bomParams = cPlayerBom.CreateBomParameters(v3, direction);
        GameObject cBom = cBomControl.DropBom(bomParams);
        cPlayerBom.AddBom(cBom);
    }

    private void AttackExplosion(){
        //Vector3 v3 = GetPos();
        //Vector3 direction = myTransform.forward;
        //DropBomと同じように実装しよう。
        //gBomControl = GameObject.Find("BomControl");
        cBomControl.CancelInvokeAndCallExplosion();
    }

    public void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Explosion"))
        {
            string materialName = other.GetComponent<Renderer>().material.name.Replace(" (Instance)", "");
            if(MaterialType != materialName){
                if(cPowerGage == null){
                    return;
                }
                int iDamage = other.GetComponent<Explosion_Base>().GetDamage();
                cPowerGage.SetDamage(iDamage);
                if(cPowerGage.IsDead()){
                    DestroySync(this.gameObject);
                }
            }
        }
    }

	protected virtual void DestroySync(GameObject g){}

    private void OnCollisionEnter(Collision collision){
        switch (collision.transform.name){
            case "Bom(Clone)":
            case "Bombigban(Clone)":
            case "BomExplode(Clone)":
                //Debug.Log(collision.transform.name);
                // ここに処理を記述
                break;
            case "Ground(Clone)":
            default:
                return;
        }

        Vector3 collisionDirectionTemp = Vector3.zero;
        Vector3 collisionDirection = Vector3.zero;

        foreach (ContactPoint contact in collision.contacts)
        {
            collisionDirectionTemp += contact.point - transform.position;
        }

        collisionDirectionTemp /= collision.contacts.Length;

        //Debug.Log("X :" + collisionDirectionTemp.x + "Z :" + collisionDirectionTemp.z);
        float threshold = 0.3f; // 閾値
        // x軸方向の判定
        if (Mathf.Abs(collisionDirectionTemp.x) > threshold)
        {
            collisionDirection.x = collisionDirectionTemp.x;
        }
        else if (Mathf.Abs(collisionDirectionTemp.z) > threshold)
        {
            collisionDirection.z = collisionDirectionTemp.z;
        }
        // Bomオブジェクトに方向を伝える
        collision.transform.GetComponent<Bom_Base>().SetMoveDirection(collisionDirection * 1.5F);
    }

    public void OnCollisionExit(Collision col) {
    }


    protected void CreatePlayerBom(){
        cPlayerBom = new PlayerBom();
    }



    public PlayerBom GetPlayerBom(){
        return cPlayerBom;
    }

    public PlayerAction GetPlayerAction(){
        return cPlayerAction;
    }

    public void Wall(){
/*
        if(iViewID != GetComponent<PhotonView>().ViewID){
            return;
        }
        
        cField.SetBrokenTrriger(true);

        photonView.RPC(nameof(SetIsTrigger), RpcTarget.All, true);
        GetComponent<Collider>().isTrigger = false;        
*/
    }
 
    public void HeartUp(int iHeart){
		//Debug.Log(gameObject);
        cPowerGage.HeartUp(iHeart);
    }


}
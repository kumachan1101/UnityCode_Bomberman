
using UnityEngine;
[System.Serializable]
public class PlayerAction
{
    [SerializeField]protected PlayerMovement playerMovement;
    private PlayerInput playerInput;
    protected PlayerMaterial playerMaterial;

    protected Transform myTransform;
    protected Field_Block_Base cField;
    protected Field_Player_Base cField_Player;
    protected Vector3 LastV3;
    protected bool canMove = true;
    protected Library_Base cLibrary;

    public PlayerAction(ref Rigidbody rb, ref Transform tf)
    {
        cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
        cField_Player = GameObject.Find("Field").GetComponent<Field_Player_Base>();
        myTransform = tf;

        playerMovement = new PlayerMovement(rb, tf);
        playerInput = new PlayerInput();
        playerMaterial = new PlayerMaterial();

        LastV3 = myTransform.position;
    }


    virtual public void MoveUp()
    {
        PerformPlayerAction(Vector3.forward);
    }

    virtual public void MoveDown()
    {
        PerformPlayerAction(Vector3.back);
    }

    virtual public void MoveRight()
    {
        PerformPlayerAction(Vector3.right);
    }

    virtual public void MoveLeft()
    {
        PerformPlayerAction(Vector3.left);
    }
	protected virtual void CanMove(){
	}
	

     protected virtual void UpdatePlayerMovement()
    {
        if (playerInput.pushBtnUp)
        {
			MoveUp();
        }
        else if (playerInput.pushBtnDown)
        {
            MoveDown();
        }
        else if (playerInput.pushBtnLeft)
        {
            MoveLeft();
        }
        else if (playerInput.pushBtnRight)
        {
            MoveRight();
        }
		else{
			MoveClear();
		}
    }
    public void SetMaterialType(string sParamMaterialType)
    {
		if(sParamMaterialType == null){
			Debug.Log("MaterialType is null");
			return;
		}
        playerMaterial.SetMaterialType(sParamMaterialType);
    }
    public void UpdateMovement()
    {
        playerInput.UpdateInput();
        UpdatePlayerMovement();
		CanMove();
    }

    public void MoveClear()
    {
        playerInput.ClearInput();
        playerMovement.MoveClear();
    }

    public void PerformPlayerAction(Vector3 moveDirection)
    {
        MoveClear();
        playerMovement.Move(moveDirection);
    }

    public void Move(Vector3 moveDirection){
        playerMovement.Move(moveDirection);
    }
    public void SpeedUp()
    {
		playerMovement.SpeedUp();
    }

    private GameObject GetPlayerGameObject()
    {
        string name = cField_Player.GetName();
        GameObject gPlayer = GameObject.Find(name);
        return gPlayer;
    }

    protected Player_Base GetPlayerComponent(GameObject gPlayer)
    {
        Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
        return cPlayer;
    }


}

using UnityEngine;
using Photon.Pun;
[System.Serializable]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]private float moveSpeed = 3.0f;
    private PlayerAnimation playerAnimation;

    public void Awake()
    {
        playerAnimation = this.gameObject.AddComponent<PlayerAnimation>();
    }
	public void Move(Vector3 direction)
	{
		transform.position += direction * moveSpeed * Time.deltaTime;
		Quaternion targetRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
		playerAnimation.SetWalking(true);
	}

    public void MoveClear()
    {
		playerAnimation.SetWalking(false);
    }

    public void SpeedUp()
    {
        if (moveSpeed < 10)
        {
            moveSpeed += 1f;
        }
    }

    public Vector3 GetCurrentPos(){
        return transform.position;
    }
}

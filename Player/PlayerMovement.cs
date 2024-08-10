using UnityEngine;
[System.Serializable]
public class PlayerMovement
{
    private Rigidbody rigidBody;
    private Transform myTransform;
    public float moveSpeed = 3.0f;
    private PlayerAnimation playerAnimation;

    public PlayerMovement(Rigidbody rb, Transform tf)
    {
        rigidBody = rb;
        myTransform = tf;
		playerAnimation = new PlayerAnimation(tf);

    }
	public void Move(Vector3 direction)
	{
		//rigidBody.velocity = direction * moveSpeed;
		myTransform.position += direction * moveSpeed * Time.deltaTime;
		Quaternion targetRotation = Quaternion.LookRotation(direction);
		myTransform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
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
}

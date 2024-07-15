using UnityEngine;

public class PlayerAnimation
{
    private Animator animator;

    public PlayerAnimation(Transform myTransform)
    {
        //animator = myTransform.Find("PlayerModel").GetComponent<Animator>();
		animator = myTransform.GetComponent<Animator>();
    }

    public void SetWalking(bool isWalking)
    {
        //animator.SetBool("Walking", isWalking);
		animator.SetBool("Move", isWalking);
    }
}

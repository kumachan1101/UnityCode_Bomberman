using UnityEngine;

public class PlayerAnimation:MonoBehaviour
{
    private Animator animator;
    public void Awake()
    {
      animator = this.gameObject.GetComponent<Animator>();
    }

    public void SetWalking(bool isWalking)
    {
		  animator.SetBool("Move", isWalking);
    }
}

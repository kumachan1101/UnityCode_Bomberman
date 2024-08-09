using UnityEngine;
public class Explosion_CpuMode : Explosion_Base
{
/*
	protected override void DestroySync(GameObject g){
		if (g == null)
		{
			Debug.LogWarning("Instance is null, cannot enqueue.");
			return;
		}
		//Field_Block_Base cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
		cField.EnqueueObject(g);

		//Destroy(g);
	}
*/
	protected override bool IsSync(){
		return true;
	}
	public override void SetPosition_RPC(Vector3 position)
	{
		SetPosition(position);
	}
}

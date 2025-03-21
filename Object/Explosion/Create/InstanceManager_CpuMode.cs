using UnityEngine;

public class InstanceManager_CpuMode : InstanceManager_Base
{
    public override void InstantiateInstancePool(Vector3 position)
	{
        InstantiateInstancePool_Base(position);
    }

    public override void DestroyInstancePool(GameObject instance)
    {
        gTemp = instance;
        DestroyInstancePool_Base();
    }


}

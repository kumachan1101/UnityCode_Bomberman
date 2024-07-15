using UnityEngine;

public class InstanceManager_CpuMode : InstanceManager_Base
{
    public override GameObject InstantiateInstance(Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is not set.");
            return null;
        }
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        return instance;
    }

    public override void DestroyInstance(GameObject instance)
    {
        if (instance != null)
        {
            Destroy(instance);
        }
    }
}

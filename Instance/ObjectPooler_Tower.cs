using UnityEngine;
public class ObjectPooler_Tower : ObjectPooler_Base
{
    protected override void AddExplostionComponent(GameObject instance){
        instance.AddComponent<Explosion_NotGround>();
    }

}

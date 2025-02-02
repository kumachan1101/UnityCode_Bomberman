using UnityEngine;
using System.Collections.Generic;
public class ObjectPooler_Local : ObjectPooler_Base
{
    protected override void AddExplostionComponent(GameObject instance){
        instance.AddComponent<Explosion_Base>();
    }

}

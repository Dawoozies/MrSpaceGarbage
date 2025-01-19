using Unity.Entities;
using UnityEngine;
public class DestroyAfterTimeAuthoring : MonoBehaviour
{
    public float time;
}
public class DestroyAfterTimeAuthoringBaker : Baker<DestroyAfterTimeAuthoring>
{
    public override void Bake(DestroyAfterTimeAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new DestroyAfterTime
        {
            timeLeft = authoring.time
        });
    }
}
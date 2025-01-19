using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputAuthoring : MonoBehaviour
{
}
public class InputAuthoringBaker : Baker<InputAuthoring>
{
    public override void Bake(InputAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent<MoveInput>(entity);
    }
}
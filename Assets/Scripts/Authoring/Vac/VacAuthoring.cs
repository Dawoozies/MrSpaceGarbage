using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class VacAuthoring : MonoBehaviour
{
    public bool playerVac;
}

public class VacAuthoringBaker : Baker<VacAuthoring>
{
    public override void Bake(VacAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<VacTag>(entity);
        if (authoring.playerVac)
        {
            AddComponent<PlayerTag>(entity);
        }
    }
}
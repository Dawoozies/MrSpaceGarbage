using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed;
    public float maxSlopeAngle;
    public float jumpImpulse;
    public float maxJetpackFuel;
    public float jetpackFuel;
    public float fuelUseSpeed;
    public float refuelSpeed;
}
public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<PlayerTag>(entity);
        AddComponent(entity, new PlayerMovement
        {
            moveSpeed = authoring.moveSpeed,
            maxSlopeAngle = authoring.maxSlopeAngle,
            jumpImpulse = authoring.jumpImpulse,
            maxJetpackFuel = authoring.jetpackFuel,
            jetpackFuel = authoring.jetpackFuel,
            fuelUseSpeed = authoring.fuelUseSpeed,
            refuelSpeed = authoring.refuelSpeed,
        });
    }
}
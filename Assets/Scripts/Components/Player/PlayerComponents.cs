using Latios.Psyshock;
using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public struct PlayerTag : IComponentData { }
public struct PlayerMovement : IComponentData
{
    public float moveSpeed;
    public float jumpImpulse;
    public float jumpCooldown;

    public float maxJetpackFuel;
    public float jetpackFuel;
    public float jetpackSpeed;
    public float fuelUseSpeed;
    public float refuelSpeed;
    public bool grounded;
    public float maxSlopeAngle;
}

public struct PlayerVac : IComponentData
{
    public float strength;
    public Entity vacBoxEntity;

}

public struct VacTag : IComponentData { }

public partial struct PlayerCollisionLayer : ICollectionComponent
{
    public CollisionLayer layer;

    public JobHandle TryDispose(JobHandle inputDeps) => layer.IsCreated ? layer.Dispose(inputDeps) : inputDeps;
}
public struct GroundElement : IBufferElementData
{
    public Entity groundEntity;
}
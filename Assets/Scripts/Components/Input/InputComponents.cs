using Latios;
using Latios.Psyshock;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
public enum ButtonInput
{
    None, WasPressedThisFrame, IsPressed, WasReleasedThisFrame
}
public struct ScreenToWorld : IComponentData
{
    public float3 A;
    public float3 B;
    public float3 Center;
    public float Width;
    public float Height;
}
public struct MousePos : IComponentData
{
    public float2 worldPos;
}
public struct DragMouseInput : IComponentData
{
    public EntityWith<DraggableTag> entity;
}
public struct DraggableTag : IComponentData { }
public partial struct DraggableCollisionLayer : ICollectionComponent
{
    public CollisionLayer layer;
    public JobHandle TryDispose(JobHandle inputDeps)
    {
        return layer.IsCreated ? layer.Dispose(inputDeps) : inputDeps;
    }
}
public struct DropMouseInput : IComponentData
{
    public EntityWith<DraggableTag> dragEntity;
    public EntityWith<DroppableTag> dropEntity;
}
public struct DroppableTag : IComponentData { }
public partial struct DroppableCollisionLayer : ICollectionComponent
{
    public CollisionLayer layer;
    public JobHandle TryDispose(JobHandle inputDeps)
    {
        return layer.IsCreated ? layer.Dispose(inputDeps) : inputDeps;
    }
}
public struct MoveInput : IComponentData
{
    public float2 move;
    public float jumpInput;
}
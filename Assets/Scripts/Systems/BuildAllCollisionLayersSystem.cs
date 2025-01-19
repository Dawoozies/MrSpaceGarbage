using BB;
using Latios;
using Latios.Psyshock;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct BuildAllCollisionLayersSystems : ISystem, ISystemNewScene
{
    BuildCollisionLayerTypeHandles typeHandles;
    LatiosWorldUnmanaged latiosWorld;

    EntityQuery staticEnvironmentQuery;
    EntityQuery playerQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        latiosWorld = state.GetLatiosWorldUnmanaged();
        staticEnvironmentQuery = state.Fluent().With<StaticEnvironmentTag>(true).PatchQueryForBuildingCollisionLayer().Build();
        playerQuery = state.Fluent().With<PlayerMovement>(true).PatchQueryForBuildingCollisionLayer().Build();

        typeHandles = new BuildCollisionLayerTypeHandles(ref state);
    }
    [BurstCompile]
    public void OnNewScene(ref SystemState state)
    {
        latiosWorld.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld(new StaticEnvironmentCollisionLayer());
        latiosWorld.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld(new PlayerCollisionLayer());

    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        typeHandles.Update(ref state);
        CollisionLayerSettings settings = BuildCollisionLayerConfig.defaultSettings;
        state.Dependency = Physics.BuildCollisionLayer(staticEnvironmentQuery, in typeHandles).WithSettings(settings)
            .ScheduleParallel(out CollisionLayer staticEnvironmentLayer, Allocator.Persistent, state.Dependency);
        latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new StaticEnvironmentCollisionLayer
        {
            layer = staticEnvironmentLayer
        });

        state.Dependency = Physics.BuildCollisionLayer(playerQuery, in typeHandles).WithSettings(settings)
            .ScheduleParallel(out CollisionLayer playerLayer, Allocator.Persistent, state.Dependency);
        latiosWorld.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new PlayerCollisionLayer
        {
            layer = playerLayer
        });

    }
}

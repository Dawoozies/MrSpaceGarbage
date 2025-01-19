using Latios;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Latios.Transforms;
using static Unity.Entities.SystemAPI;

[BurstCompile]
public partial struct PlayerVacSystem : ISystem
{
    LatiosWorldUnmanaged latiosWorld;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        latiosWorld = state.GetLatiosWorldUnmanaged();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new VacBoxMove
        {
            sbe = latiosWorld.sceneBlackboardEntity,
            transformLookup = GetComponentLookup<WorldTransform>(false),
            aimInputLookup = GetComponentLookup<AimInput>(true)
        }.Schedule();
    }
    [BurstCompile]
    [WithAll(typeof(PlayerTag), typeof(VacTag))]
    partial struct VacBoxMove : IJobEntity
    {
        public Entity sbe;
        public ComponentLookup<WorldTransform> transformLookup;
        [ReadOnly] public ComponentLookup<AimInput> aimInputLookup;
        public void Execute(Entity vacEntity)
        {
            var vacTransform = transformLookup[vacEntity];
            var aimInput = aimInputLookup[sbe];
            vacTransform.worldTransform.rotation = quaternion.AxisAngle(math.forward(), aimInput.aimAngle);
            transformLookup[vacEntity] = vacTransform;
        }
    }
}

using Latios;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct DestroyAfterTimeSystem : ISystem
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
        var dcb = latiosWorld.syncPoint.CreateDestroyCommandBuffer().AsParallelWriter();
        new DestroyAfterTimeJob
        {
            globalDeltaTime = SystemAPI.Time.DeltaTime,
            dcb = dcb,
        }.ScheduleParallel();
    }
    partial struct DestroyAfterTimeJob : IJobEntity
    {
        public float globalDeltaTime;
        public DestroyCommandBuffer.ParallelWriter dcb;
        public void Execute(Entity entity, [ChunkIndexInQuery] int chunkIndexInQuery, ref DestroyAfterTime destroyAfterTime)
        {
            if (destroyAfterTime.timeLeft > 0)
            {
                destroyAfterTime.timeLeft -= globalDeltaTime;
                return;
            }

            dcb.Add(entity, chunkIndexInQuery);
        }
    }
}

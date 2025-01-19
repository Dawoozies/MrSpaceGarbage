using Latios;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Latios.Transforms;
using BB;
using static Unity.Entities.SystemAPI;
using Latios.Psyshock;
[BurstCompile]
public partial struct PlayerMoveSystem : ISystem
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
        new PlayerMove
        {
            sbe = latiosWorld.sceneBlackboardEntity,
            moveInputLookup = GetComponentLookup<MoveInput>(true),
            deltaTime = Time.DeltaTime,
        }.Schedule();

        var playerLayer = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<PlayerCollisionLayer>().layer;
        var staticEnvironmentLayer = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<StaticEnvironmentCollisionLayer>().layer;
        var playerGroundCheckJob = new PlayerGroundCollisionBuffer
        {
            bodyLookup = GetComponentLookup<RigidBody>(false),
            playerMovementLookup = GetComponentLookup<PlayerMovement>(false),
            groundBufferLookup = GetBufferLookup<GroundElement>(false),
        };
        state.Dependency = Physics.FindPairs(playerLayer, staticEnvironmentLayer, playerGroundCheckJob).ScheduleParallel(state.Dependency);

        new PlayerGroundBufferCheck
        {
        }.ScheduleParallel();
    }
    [BurstCompile]
    partial struct PlayerMove : IJobEntity
    {
        public Entity sbe;
        [ReadOnly] public ComponentLookup<MoveInput> moveInputLookup;
        public float deltaTime;
        public void Execute(ref RigidBody rigidBody, ref PlayerMovement playerMovement)
        {
            var moveInput = moveInputLookup[sbe];
            rigidBody.velocity.linear.x = moveInput.move.x * playerMovement.moveSpeed;
            
            if(playerMovement.jumpCooldown <= 0f && playerMovement.grounded && moveInput.jumpInput > 0f)
            {
                rigidBody.velocity.linear.y += playerMovement.jumpImpulse;
                playerMovement.jumpCooldown = 0.02f;
            }
            if(playerMovement.jumpCooldown > 0f)
            {
                if(!playerMovement.grounded)
                {
                    playerMovement.jumpCooldown = 0f;
                }
                playerMovement.jumpCooldown -= deltaTime;
            }

            if (!playerMovement.grounded && moveInput.jumpInput > 0 && playerMovement.jetpackFuel > 0)
            {
                rigidBody.velocity.linear.y += playerMovement.jetpackSpeed * deltaTime;
                playerMovement.jetpackFuel -= deltaTime * playerMovement.fuelUseSpeed;
            }

            if (moveInput.jumpInput <= 0 && playerMovement.jetpackFuel < playerMovement.maxJetpackFuel)
            {
                playerMovement.jetpackFuel += deltaTime * playerMovement.refuelSpeed;
            }
        }
    }
    struct PlayerGroundCollisionBuffer : IFindPairsProcessor
    {
        public PhysicsComponentLookup<RigidBody> bodyLookup;
        public PhysicsComponentLookup<PlayerMovement> playerMovementLookup;
        public PhysicsBufferLookup<GroundElement> groundBufferLookup; 
        public void Execute(in FindPairsResult result)
        {
            var playerRigidBody = bodyLookup[result.entityA];
            var playerMovement = playerMovementLookup[result.entityA];
            var groundBuffer = groundBufferLookup[result.entityA];
            var maxDistance = UnitySim.MotionExpansion.GetMaxDistance(in playerRigidBody.motionExpansion);
            if (Physics.DistanceBetween(result.colliderA, result.transformA, result.colliderB, result.transformB, maxDistance, out var hitData))
            {
                var angleBetween = math.acos(math.dot(math.up(), hitData.normalB)) * math.TODEGREES;
                if (angleBetween <= playerMovement.maxSlopeAngle)
                {
                    groundBuffer.Add(new GroundElement { groundEntity = result.entityB});
                    return;
                }
            }
        }
    }
    [BurstCompile]
    partial struct PlayerGroundBufferCheck : IJobEntity
    {
        public void Execute(ref DynamicBuffer<GroundElement> groundBuffer, ref PlayerMovement playerMovement)
        {
            if (groundBuffer.Length <= 0)
            {
                playerMovement.grounded = false;
                return;
            }

            playerMovement.grounded = true;
            groundBuffer.Clear();
        }
    }
}

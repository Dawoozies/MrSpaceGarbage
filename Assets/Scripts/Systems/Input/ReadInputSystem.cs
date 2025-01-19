using Latios;
using Latios.Psyshock;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Physics = Latios.Psyshock.Physics;
using Ray = Latios.Psyshock.Ray;
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial class ReadInputSystem : SubSystem
{
    Camera mainCamera;
    const float maxCastDist = 250f;
    public override void OnNewScene()
    {
        mainCamera = Camera.main;
    }
    protected override void OnUpdate()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;

        var move = float2.zero;
        var jumpInput = 0f;

        if(gamepad != null)
        {
            move = gamepad.leftStick.ReadValue();
            jumpInput = gamepad.aButton.ReadValue();
        }
        else
        {
            var leftInput = -keyboard.aKey.ReadValue();
            var rightInput = keyboard.dKey.ReadValue();
            var upInput = keyboard.wKey.ReadValue();
            var downInput = -keyboard.sKey.ReadValue();
            move.x = leftInput + rightInput;
            move.y = upInput > 0 ? upInput : downInput;
            jumpInput = keyboard.spaceKey.ReadValue();
        }

        new MoveInputJob
        {
            move = move,
            jumpInput = jumpInput,
        }.ScheduleParallel();
    }
    [BurstCompile]
    partial struct MoveInputJob : IJobEntity
    {
        public float2 move;
        public float jumpInput;
        public void Execute(ref MoveInput moveInput)
        {
            moveInput.move = move;
            moveInput.jumpInput = jumpInput;
        }
    }
}

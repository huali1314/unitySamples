using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

//public class MoveSystem : ComponentSystem
//{
//    public static void TimeConsumingFunction()
//    {
//        float value = 0f;
//        for (int i = 0; i < 5000; i++)
//        {
//            value = math.exp10(math.sqrt(value));
//        }
//    }
//    protected override void OnUpdate()
//    {
//        Entities.ForEach((ref Translation translation, ref MoveSpeedComponent moveSpeedComponent) =>
//        {
//            translation.Value.y += moveSpeedComponent.moveSpeed * Time.deltaTime;
//            if (translation.Value.y > 5f)
//            {
//                moveSpeedComponent.moveSpeed = -math.abs(moveSpeedComponent.moveSpeed);
//            }
//            if (translation.Value.y < -5f)
//            {
//                moveSpeedComponent.moveSpeed = +math.abs(moveSpeedComponent.moveSpeed);
//            }
//            TimeConsumingFunction();
//        });
//    }
//}

public class MoveSystem : JobComponentSystem
{
    public static void TimeConsumingFunction()
    {
        float value = 0f;
        for (int i = 0; i < 5000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
    [BurstCompile]
    private struct MoveJob : IJobForEach<Translation, MoveSpeedComponent>
    {
        public float deltaTime;

        public void Execute(ref Translation c0, ref MoveSpeedComponent c1)
        {
            c0.Value.y += c1.moveSpeed * deltaTime;
            if (c0.Value.y > 5f)
            {
                c1.moveSpeed = -math.abs(c1.moveSpeed);
            }
            if (c0.Value.y < -5f)
            {
                c1.moveSpeed = +math.abs(c1.moveSpeed);
            }
            TimeConsumingFunction();
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        MoveJob moveJob = new MoveJob
        {
            deltaTime = Time.deltaTime
        };
        return moveJob.Schedule(this, inputDeps);
    }
}
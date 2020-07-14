using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
//public class FinderMoveToTargetSystem : ComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        Entities.WithAll<HasTargetComponent>().ForEach((Entity entity, ref HasTargetComponent hasTargetComponent, ref Translation translation) =>
//        {
//            if (!World.Active.EntityManager.Exists(hasTargetComponent.targetEntity))
//            {
//                PostUpdateCommands.RemoveComponent(entity, typeof(HasTargetComponent));
//                return;
//            }
//            Translation targetTranslation = World.Active.EntityManager.GetComponentData<Translation>(hasTargetComponent.targetEntity);
//            float3 targetDir = math.normalize(targetTranslation.Value - translation.Value);
//            float moveSpeed = UnityEngine.Random.Range(0f, 5f);
//            translation.Value += targetDir * moveSpeed * Time.deltaTime;
//            if (math.distance(targetTranslation.Value, translation.Value) < 0.2f)
//            {
//                PostUpdateCommands.DestroyEntity(hasTargetComponent.targetEntity);
//                PostUpdateCommands.RemoveComponent(entity, typeof(HasTargetComponent));
//            }
//        });
//    }
//}
public class FindMoveToTargetJobSystem : JobComponentSystem
{
    [RequireComponentTag(typeof(HasTargetComponent))]
    //[BurstCompile]
    public struct FinderMoveToTargetJob : IJobForEachWithEntity<Translation>
    {
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public float deltaTime;
        public float moveSpeed;
        //在任务完成的时候释放内存（解除分配内存）
        [DeallocateOnJobCompletion]
        public NativeArray<HasTargetComponent> targetArray;
        public void Execute(Entity entity, int index, ref Translation finderTranslation)
        {
            if (targetArray[index].targetEntity != Entity.Null && targetArray[index].finderEntity == entity)
            {
                if (!World.Active.EntityManager.Exists(targetArray[index].targetEntity))
                {
                    entityCommandBuffer.RemoveComponent(index, entity, typeof(HasTargetComponent));
                    return;
                }
                float3 targetDir = math.normalize(targetArray[index].targetPosition - finderTranslation.Value);

                finderTranslation.Value += targetDir * moveSpeed * deltaTime;
                if (math.distance(targetArray[index].targetPosition, finderTranslation.Value) < 0.2f)
                {
                    entityCommandBuffer.DestroyEntity(index, targetArray[index].targetEntity);
                    entityCommandBuffer.RemoveComponent(index, entity, typeof(HasTargetComponent));
                }
            }
        }
    }
    EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;
    protected override void OnCreate()
    {
        endSimulationEntityCommandBufferSystem = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityQuery entityQuery = GetEntityQuery(typeof(HasTargetComponent), ComponentType.ReadOnly<Translation>());
        NativeArray<HasTargetComponent> targetArray = entityQuery.ToComponentDataArray<HasTargetComponent>(Allocator.TempJob);
        FinderMoveToTargetJob finderMoveToTargetJob = new FinderMoveToTargetJob
        {
            moveSpeed = UnityEngine.Random.Range(1, 3),
            deltaTime = Time.deltaTime,
            targetArray = targetArray,
            entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        JobHandle jobHandle = finderMoveToTargetJob.Schedule(this, inputDeps);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}

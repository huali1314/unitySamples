using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;
//public class FindersSystem : ComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        Entities.WithNone<HasTargetComponent>().WithAll<FinderTag>().ForEach((Entity finderEntity,ref Translation finderTranslation) =>
//        {
//            Debug.Log("Finder===" + finderEntity);
//            float3 finderPos = finderTranslation.Value;
//            Entity clostestTargetEntity = Entity.Null;
//            float3 clostestTargetEntityPos = float3.zero;
//            Entities.WithAll<TargetTag>().ForEach((Entity targetEntity,ref Translation targetTranslation) =>
//            {
//                Debug.Log("Target=====" + targetEntity);
//                if (clostestTargetEntity == Entity.Null)
//                {
//                    clostestTargetEntity = targetEntity;
//                    clostestTargetEntityPos = targetTranslation.Value;
//                }
//                else
//                {
//                    if (math.distance(finderPos, targetTranslation.Value) < math.distance(finderPos, clostestTargetEntityPos))
//                    {
//                        clostestTargetEntity = targetEntity;
//                        clostestTargetEntityPos = targetTranslation.Value;
//                    }
//                }
//            });
//            if (clostestTargetEntity != Entity.Null)
//            {
//                EntityManager.AddComponentData<HasTargetComponent>(finderEntity, new HasTargetComponent
//                {
//                    targetEntity = clostestTargetEntity
//                }) ;
//                Debug.DrawLine(finderPos, clostestTargetEntityPos);
//            }
//        });
//    }
//}


//public class FindTargetJobSystem : JobComponentSystem
//{
//    public struct EntityWithPosition {
//        public Entity targetEntity;
//        public float3 targetPosition;
//    }
//    //包含组件FinderTag
//    [RequireComponentTag(typeof(FinderTag))]
//    //不包含组件HasTargetComponent
//    [ExcludeComponent(typeof(HasTargetComponent))]
//    //[BurstCompile]
//    public struct FindTargetJob : IJobForEachWithEntity<Translation>
//    {

//        public EntityCommandBuffer.Concurrent entityCommandBuffer;
//        //在任务完成的时候释放内存（解除分配内存）
//        [DeallocateOnJobCompletion][ReadOnly]
//        public NativeArray<EntityWithPosition> targetArray;

//        public void Execute(Entity entity, int index, [ReadOnly]ref Translation finderTranslation)
//        {
//            float3 finderPos = finderTranslation.Value;
//            Entity clostestTargetEntity = Entity.Null;
//            float3 clostestTargetEntityPos = float3.zero;
//            for(int i = 0;i< targetArray.Length;i++)
//            {
//                EntityWithPosition entityWithPosition = targetArray[i];
//                if (clostestTargetEntity == Entity.Null)
//                {
//                    clostestTargetEntity = entityWithPosition.targetEntity;
//                    clostestTargetEntityPos = entityWithPosition.targetPosition;
//                }
//                else
//                {
//                    if (math.distance(finderPos, entityWithPosition.targetPosition) < math.distance(finderPos, clostestTargetEntityPos))
//                    {
//                        clostestTargetEntity = entityWithPosition.targetEntity;
//                        clostestTargetEntityPos = entityWithPosition.targetPosition;
//                    }
//                }
//            }
//            if (clostestTargetEntity != Entity.Null)
//            {
//                entityCommandBuffer.AddComponent<HasTargetComponent>(index,entity, new HasTargetComponent
//                {
//                    finderEntity = entity,
//                    targetEntity = clostestTargetEntity,
//                    targetPosition = clostestTargetEntityPos
//                });
//                //Debug.DrawLine(finderPos, clostestTargetEntityPos);
//            }
//        }
//    }



//    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;
//    protected override void OnCreate()
//    {
//        endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//    }
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        EntityQuery targetQuery = GetEntityQuery(typeof(TargetTag), ComponentType.ReadOnly<Translation>());
//        NativeArray<Entity> targetEntityArray = targetQuery.ToEntityArray(Allocator.TempJob);
//        NativeArray<Translation> targetTranslationArray = targetQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
//        NativeArray<EntityWithPosition> targetArray = new NativeArray<EntityWithPosition>(targetEntityArray.Length, Allocator.TempJob);
//        for (int i = 0; i < targetEntityArray.Length; i++)
//        {
//            targetArray[i] = new EntityWithPosition
//            {
//                targetEntity = targetEntityArray[i],
//                targetPosition = targetTranslationArray[i].Value
//            };
//        }
//        targetEntityArray.Dispose();
//        targetTranslationArray.Dispose();
//        FindTargetJob findTargetJob = new FindTargetJob
//        {
//            targetArray = targetArray,
//            entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
//        };
//        JobHandle jobHandle = findTargetJob.Schedule(this,inputDeps);
//        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
//        return jobHandle;
//    }
//}

public class FindTargetJobSystem : JobComponentSystem
{
    public struct EntityWithPosition
    {
        public Entity targetEntity;
        public float3 targetPosition;
    }
    //包含组件FinderTag
    [RequireComponentTag(typeof(FinderTag))]
    //不包含组件HasTargetComponent
    [ExcludeComponent(typeof(HasTargetComponent))]
    [BurstCompile]
    public struct FindTargetJob : IJobForEachWithEntity<Translation>
    {

        //public EntityCommandBuffer.Concurrent entityCommandBuffer;
        //在任务完成的时候释放内存（解除分配内存）
        [DeallocateOnJobCompletion]
        [ReadOnly]
        public NativeArray<EntityWithPosition> targetArray;
        public NativeArray<Entity> closestTargetArray;
        public NativeArray<float3> closestTargetPosArray;
        public void Execute(Entity entity, int index, [ReadOnly]ref Translation finderTranslation)
        {
            float3 finderPos = finderTranslation.Value;
            Entity clostestTargetEntity = Entity.Null;
            float3 clostestTargetEntityPos = float3.zero;
            for (int i = 0; i < targetArray.Length; i++)
            {
                EntityWithPosition entityWithPosition = targetArray[i];
                if (clostestTargetEntity == Entity.Null)
                {
                    clostestTargetEntity = entityWithPosition.targetEntity;
                    clostestTargetEntityPos = entityWithPosition.targetPosition;
                }
                else
                {
                    if (math.distance(finderPos, entityWithPosition.targetPosition) < math.distance(finderPos, clostestTargetEntityPos))
                    {
                        clostestTargetEntity = entityWithPosition.targetEntity;
                        clostestTargetEntityPos = entityWithPosition.targetPosition;
                    }
                }
            }
            closestTargetArray[index] = clostestTargetEntity;
            closestTargetPosArray[index] = clostestTargetEntityPos ;
            //if (clostestTargetEntity != Entity.Null)
            //{
            //    entityCommandBuffer.AddComponent<HasTargetComponent>(index, entity, new HasTargetComponent
            //    {
            //        finderEntity = entity,
            //        targetEntity = clostestTargetEntity,
            //        targetPosition = clostestTargetEntityPos
            //    });
            //    //Debug.DrawLine(finderPos, clostestTargetEntityPos);
            //}
        }
    }
    //包含组件FinderTag
    [RequireComponentTag(typeof(FinderTag))]
    //不包含组件HasTargetComponent
    [ExcludeComponent(typeof(HasTargetComponent))]

    //[BurstCompile]这里使用爆发编译器会报错
    public struct AddComponentJob : IJobForEachWithEntity<Translation>
    {
        //任务完成后 释放内存
        [DeallocateOnJobCompletion]
        //只读
        [ReadOnly]
        public NativeArray<Entity> closestTargetArray;
        [DeallocateOnJobCompletion]
        [ReadOnly]
        public NativeArray<float3> closestTargetPosArray;
        //
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public void Execute(Entity entity, int index, ref Translation translation)
        {
            if (closestTargetArray[index] != Entity.Null)
            {
                entityCommandBuffer.AddComponent(index, entity, new HasTargetComponent
                {
                    finderEntity = entity,
                    targetEntity = closestTargetArray[index],
                    targetPosition = closestTargetPosArray[index]
                });
            }
        }
    }

    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;
    protected override void OnCreate()
    {
        endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityQuery targetQuery = GetEntityQuery(typeof(TargetTag), ComponentType.ReadOnly<Translation>());
        NativeArray<Entity> targetEntityArray = targetQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> targetTranslationArray = targetQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<EntityWithPosition> targetArray = new NativeArray<EntityWithPosition>(targetEntityArray.Length, Allocator.TempJob);
        for (int i = 0; i < targetEntityArray.Length; i++)
        {
            targetArray[i] = new EntityWithPosition
            {
                targetEntity = targetEntityArray[i],
                targetPosition = targetTranslationArray[i].Value
            };
        }
        //本地容器使用完成后释放内存
        targetEntityArray.Dispose();
        targetTranslationArray.Dispose();
        //获取所有带有FinderTag组件，但没有HasTargetComponent组件的实体
        EntityQuery entityQuery = GetEntityQuery(typeof(FinderTag), ComponentType.Exclude<HasTargetComponent>());

        NativeArray<Entity> closestTargetArray = new NativeArray<Entity>(entityQuery.CalculateEntityCount(), Allocator.TempJob);

        NativeArray<float3> closestTargetPosArray = new NativeArray<float3>(entityQuery.CalculateEntityCount(), Allocator.TempJob);
        FindTargetJob findTargetJob = new FindTargetJob
        {
            targetArray = targetArray,
            closestTargetArray = closestTargetArray,
            closestTargetPosArray = closestTargetPosArray
            //entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        JobHandle jobHandle = findTargetJob.Schedule(this, inputDeps);

        AddComponentJob addComponentJob = new AddComponentJob
        {
            closestTargetArray = closestTargetArray,
            closestTargetPosArray = closestTargetPosArray,
            entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        jobHandle = addComponentJob.Schedule(this, jobHandle);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}

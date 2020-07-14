using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
//public class LevelSystem : ComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        //刷新所有实体数据
//        Entities.ForEach((ref LevelComponent levelComponent) =>
//        {
//            levelComponent.level += 1f * Time.deltaTime;
//            //Debug.Log(levelComponent.level);
//        });
//    }
//}

public class LevelSystem : JobComponentSystem
{
    [BurstCompile]
    public struct LevelJob : IJobForEach<LevelComponent>
    {
        public float deltaTime;
        public void Execute(ref LevelComponent c0)
        {
            c0.level += 1f * deltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        LevelJob levelJob = new LevelJob
        {
            deltaTime = Time.deltaTime
        };
        return levelJob.Schedule(this, inputDeps);
    }
}

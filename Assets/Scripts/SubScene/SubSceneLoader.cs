using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Scenes;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Burst;
using UnityEngine.SceneManagement;
public class SubSceneSystem : ComponentSystem
{
    
    protected SubSceneStreamingSystem subSceneStreamingSystem;
    protected override void OnCreate()
    {
        subSceneStreamingSystem = World.GetOrCreateSystem<SubSceneStreamingSystem>();
    }
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadSubScene(SubSceneReferences.Instance.subScene);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnloadSubScene(SubSceneReferences.Instance.subScene);
        }
    }
    private void LoadSubScene(SubScene subScene)
    {
   
        Debug.Log("subscene=====" + subScene);
       
    }
    private void UnloadSubScene(SubScene subScene)
    {
      
    }
}
//public class SubSceneSystem : JobComponentSystem
//{
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        TestJob testJob = new TestJob();
//        return testJob.Schedule(inputDeps);

//        //TestJob1 testJob1 = new TestJob1
//        //{

//        //};
//        //return testJob1.Schedule(this, inputDeps);

//    }

//    public struct TestJob : IJob
//    {
//        public void Execute()
//        {
//            for (int i = 0; i < 500000; i++)
//            {
//                math.abs(math.sqrt(i));
//            }
//        }
//    }
//    [BurstCompile]
//    public struct TestJob1 : IJobForEach<Translation>
//    {
//        public void Execute(ref Translation c0)
//        {
//            for (int i = 0; i < 100000; i++)
//            {
//                math.abs(math.sqrt(i));
//            }
//        }
//    }
//}

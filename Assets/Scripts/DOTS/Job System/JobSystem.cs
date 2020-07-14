using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;

public class JobSystem : MonoBehaviour
{ 
    [SerializeField]
    private bool useJobs;
    [SerializeField]
    private Transform pfZombie;
    private List<Zombie> zombieList;
    // Start is called before the first frame update
    void Start()
    {
        
        zombieList = new List<Zombie>();
        for (int i = 0; i < 1000; i++)
        {
            Transform zombieTransform = Instantiate(pfZombie, new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f), 0f), Quaternion.identity);
            zombieList.Add(new Zombie
            {
                transform = zombieTransform,
                moveY = UnityEngine.Random.Range(1f, 2f)
            });

        }
    }

    // Update is called once per frame
    void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs)
        {
            //Alloctor类型：Allocator.Temp(申请速度最快，在一帧之内使用的数据)，Allocator.TempJob（申请速度中等，在4帧内使用的数据），Allocator.Persistent（申请速度最慢，在整个应用程序生命周期中使用的数据）
            //NativeContainer:NativeArray(数组),NativeList（可变大小数组）,NativeHashMap（键值对）,NativeMutilHashMap（每个键有多个值）,NativeQueue（先进先出队列），存在于Unity.Collections命名空间下
            NativeArray<float3> positionArray = new NativeArray<float3>(zombieList.Count, Allocator.TempJob);
            NativeArray<float> moveYArray = new NativeArray<float>(zombieList.Count, Allocator.TempJob);
            for (int i = 0; i < zombieList.Count; i++)
            {
                positionArray[i] = zombieList[i].transform.position;
                moveYArray[i] = zombieList[i].moveY;
            }
            ReallyToughParalleJob reallyToughParalleJob = new ReallyToughParalleJob
            {
                deltaTime = Time.deltaTime,
                positionArray = positionArray,
                moveYArray = moveYArray
            };
            JobHandle jobHandle = reallyToughParalleJob.Schedule(zombieList.Count, 1);
            jobHandle.Complete();
            for (int i = 0; i < zombieList.Count; i++)
            {
                zombieList[i].transform.position = positionArray[i];
                zombieList[i].moveY = moveYArray[i];
            }
            positionArray.Dispose();
            moveYArray.Dispose();
            //TransformAccessArray transformAccessArray = new TransformAccessArray(zombieList.Count);
            //NativeArray<float> moveYArray = new NativeArray<float>(zombieList.Count, Allocator.TempJob);
            //for (int i = 0; i < zombieList.Count; i++)
            //{
            //    moveYArray[i] = zombieList[i].moveY;
            //    transformAccessArray.Add(zombieList[i].transform);
            //}
            //ReallyToughParalleJobTransform reallyToughParalleJobTransform = new ReallyToughParalleJobTransform
            //{
            //    deltaTime = Time.deltaTime,
            //    moveYArray = moveYArray
            //};
            //JobHandle jobHandle = reallyToughParalleJobTransform.Schedule(transformAccessArray);
            //jobHandle.Complete();
            //for (int i = 0; i < zombieList.Count; i++)
            //{
            //    zombieList[i].moveY = moveYArray[i];
            //}
            //moveYArray.Dispose();
            //transformAccessArray.Dispose();
        }
        else
        {
            foreach (Zombie zombie in zombieList)
            {
                zombie.transform.position += new Vector3(0, zombie.moveY * Time.deltaTime);
                if (zombie.transform.position.y > 5f)
                {
                    zombie.moveY = -math.abs(zombie.moveY);
                }
                if (zombie.transform.position.y < -5f)
                {
                    zombie.moveY = +math.abs(zombie.moveY);
                }
                float value = 0f;
                for (int i = 0; i < 1000; i++)
                {
                    value = math.exp10(math.sqrt(value));
                }

            }
        }
        

        /*
        if (useJobs)
        {
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle handle = ReallyToughTaskJob();
                jobHandleList.Add(handle);
            }
            JobHandle.CompleteAll(jobHandleList);
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                ReallyToughTask();
            }
        }*/
        
        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000 + "ms");
    }
    private void ReallyToughTask() {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
    private JobHandle ReallyToughTaskJob()
    {

        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }

}
//Job接口：IJob（基础Job接口）IJobParalleFor（并行Job接口）IJobParalleForTransform（针对操作实体Translation组件的Job接口）等
[BurstCompile]
public struct ReallyToughJob:IJob
{

    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}
[BurstCompile]
public struct ReallyToughParalleJob : IJobParallelFor
{
    public NativeArray<float3> positionArray;
    public NativeArray<float> moveYArray;
    [ReadOnly] public float deltaTime;
    public void Execute(int index)
    {
        positionArray[index] += new float3(0, moveYArray[index] * deltaTime, 0f);
        if (positionArray[index].y > 5f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }
        if (positionArray[index].y < -5f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }
        float value = 0f;
        for (int i = 0; i < 1000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }

    }
}
[BurstCompile]
public struct ReallyToughParalleJobTransform : IJobParallelForTransform
{
    public NativeArray<float> moveYArray;
    [ReadOnly] public float deltaTime;
    public void Execute(int index, TransformAccess transform)
    {
        transform.position += new Vector3(0, moveYArray[index] * deltaTime, 0f);
        if (transform.position.y > 5f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }
        if (transform.position.y < -5f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }
        float value = 0f;
        for (int i = 0; i < 1000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }

    }
}
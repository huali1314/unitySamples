using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;

public class Testing : MonoBehaviour
{
    [SerializeField]
    private Mesh mesh;
    [SerializeField]
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;
        //实体Entity上包含的所有组件
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(LevelComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(MoveSpeedComponent)
            );
        //存储所有实体的本地数组
        NativeArray<Entity> entityArray = new NativeArray<Entity>(5000, Allocator.Temp);
        //使用实体组件类型和实体数组创建实体
        entityManager.CreateEntity(entityArchetype, entityArray);
        //给每个实体添加组件数据
        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity,
                new LevelComponent
                {
                    level = Random.Range(10,20)
                });
            entityManager.SetComponentData(entity,
                new MoveSpeedComponent
                {
                    moveSpeed = Random.Range(1f, 2f)
                });
            entityManager.SetComponentData(entity,
                new Translation
                {
                    Value = new Unity.Mathematics.float3(Random.Range(-8,8f),Random.Range(-5,5f),0)
                });
            //添加共享组件数据
            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = mesh,
                material = material
            });
        }
        //释放实体数组
        entityArray.Dispose();
    }
}

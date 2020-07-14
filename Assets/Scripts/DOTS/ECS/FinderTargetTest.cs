using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class FinderTargetTest : MonoBehaviour
{
    [SerializeField]
    private Mesh mesh;
    [SerializeField]
    private Material finderMaterial;
    [SerializeField]
    private Material targetMaterial;
    EntityManager entityManager;
    private float spawnTargetTime;
    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.Active.EntityManager;
        for (int i = 0; i < 2; i++)
        {
            SpawnFinderEntities();
        }
       
    }
    private void Update()
    {
        spawnTargetTime -= Time.deltaTime;
        if (spawnTargetTime < 0)
        {
            spawnTargetTime = 2f;
            for (int i = 0; i < 5; i++)
            {
                SpawnTargetEntities();
            }
        }
    }
    private void SpawnFinderEntities()
    {
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Scale),
            typeof(FinderTag)
        );
       Entity finderEntity = entityManager.CreateEntity(entityArchetype);
       SetEntityComponentData(finderEntity, new float3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f), 0), mesh, finderMaterial,0.5f);

    }
    private void SpawnTargetEntities()
    {
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Scale),
            typeof(TargetTag)
        );
        Entity targetEntity = entityManager.CreateEntity(entityArchetype);
        SetEntityComponentData(targetEntity, new float3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f), 0), mesh, targetMaterial,0.2f);

    }
    private void SetEntityComponentData(Entity entity,float3 spawnPosition,Mesh mesh,Material material,float scale)
    {
        entityManager.SetSharedComponentData<RenderMesh>(entity,
            new RenderMesh
            {
                mesh = mesh,
                material = material
            }
        );
        entityManager.SetComponentData<Translation>(entity,
            new Translation
            {
                Value = spawnPosition
            }
        );
        entityManager.SetComponentData<Scale>(entity,
           new Scale
           {
               Value = scale
           }
       );
    }

}

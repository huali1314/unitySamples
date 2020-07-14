using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.Jobs;
public class UnitMoveOrderSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Entities.ForEach((Entity entity, ref Translation translation) =>
            {
                //Add PathFinding Params
                EntityManager.AddComponentData(entity, new PathfindingParams
                {
                    startPositon = new int2(0, 0),
                    endPosition = new int2(4, 0)
                });
            });
        }
    }
}

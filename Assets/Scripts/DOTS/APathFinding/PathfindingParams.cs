using Unity.Entities;
using Unity.Mathematics;

public struct PathfindingParams : IComponentData
{
    public int2 startPositon;
    public int2 endPosition;
          
}

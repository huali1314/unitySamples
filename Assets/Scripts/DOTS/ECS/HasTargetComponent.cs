using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct HasTargetComponent : IComponentData
{
    public Entity finderEntity;
    public Entity targetEntity;
    public float3 targetPosition;
}

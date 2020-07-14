using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
public struct FindersComponent : IComponentData
{
    public float3 position;
    public quaternion quater;
}

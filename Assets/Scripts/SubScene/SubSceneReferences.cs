using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Scenes;
public class SubSceneReferences : MonoBehaviour
{

    public static SubSceneReferences Instance { get; private set; }
    public SubScene subScene;
    private void Awake()
    {
        Instance = this;
    }
}

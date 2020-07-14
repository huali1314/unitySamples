using System;
using UnityEngine;

[Serializable]
public class UIPanelInformation : ISerializationCallbackReceiver
{
    //[HideInInspector] //在Inspector版面中隐藏public属性，与下面相比，只是隐藏，没有序不序列化的功能
    //[NonSerialized]  //在Inspector版面中隐藏public属性，并且序列化
    //[SerializeField]  //在Inspector版面中显示非public属性，并且序列化
	[NonSerialized]
    public UIPanelType panelType;
    public string panelTypeString;
    public string path;
    public void OnAfterDeserialize(){
    	//反序列化之后，将一个或多个枚举字符串表示(panelTypeString)转换成等效的枚举对象(UIPanelType)
    	UIPanelType type = (UIPanelType)Enum.Parse(typeof(UIPanelType),panelTypeString);
    	panelType = type;
    }
    public void OnBeforeSerialize(){

    }
}

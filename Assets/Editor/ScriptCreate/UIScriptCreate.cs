using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
public class UIScriptCreate
{
    [MenuItem("CustomTools/Scripts/CreateUITempleteScripts")]
    static void CreatTempleteScript()
    {
        string content = File.ReadAllText("Assets/Editor/templeteScripts/templeteUIScripts.cs");
        File.WriteAllText("Assets/Scripts/utils/UIScript.cs", content,Encoding.UTF8);
		//刷新一下资源，不然创建好文件后第一时间不会显示
		AssetDatabase.Refresh();

    }
}
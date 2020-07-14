using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using System.Xml;
using UnityEditor.SceneManagement;
#region 将地图数据导出为XML
public class ExportXML : Editor
{
    [MenuItem("CustomTools/SceneTools/ExportXML")]
    static void ExportSceneDataToXML()
    {
        string path = Application.dataPath + "/StreamingAssets/SceneXML.xml";
        if (!File.Exists(path))
        {
            File.Delete(path);
        }
        XmlDocument document = new XmlDocument();
        XmlElement root = document.CreateElement("gameObjects");
        //遍历所有已添加的scene
        foreach (UnityEditor.EditorBuildSettingsScene s in UnityEditor.EditorBuildSettings.scenes)
        {
            if (s.enabled)
            {
                string name = s.path;
                UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(name);
                XmlElement scenes = document.CreateElement("scenes");
                scenes.SetAttribute("name", name);
                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    if (obj.transform.parent == null)
                    {
                        XmlElement gameObject = document.CreateElement("gameObjects");
                        gameObject.SetAttribute("name",obj.name);
                        gameObject.SetAttribute("asset", obj.name + ".prefab");
                        XmlElement transform = document.CreateElement("transform");
                        XmlElement position = document.CreateElement("position");

                        XmlElement position_x = document.CreateElement("x");
                        position_x.InnerText = obj.transform.position.x + "";

                        XmlElement position_y = document.CreateElement("y");
                        position_y.InnerText = obj.transform.position.y + "";

                        XmlElement position_z = document.CreateElement("z");
                        position_z.InnerText = obj.transform.position.z + "";

                        transform.AppendChild(position_x);
                        transform.AppendChild(position_y);
                        transform.AppendChild(position_z);

                        XmlElement rotation = document.CreateElement("rotation");
                        XmlElement rotation_x = document.CreateElement("x");
                        rotation_x.InnerText = obj.transform.rotation.x + "";
                        XmlElement rotation_y = document.CreateElement("y");
                        rotation_y.InnerText = obj.transform.rotation.y + "";
                        XmlElement rotation_z = document.CreateElement("z");
                        rotation_z.InnerText = obj.transform.rotation.z + "";
                        rotation.AppendChild(rotation_x);
                        rotation.AppendChild(rotation_y);
                        rotation.AppendChild(rotation_z);

                        XmlElement scale = document.CreateElement("scale");
                        XmlElement scale_x = document.CreateElement("x");
                        scale_x.InnerText = obj.transform.localScale.x + "";
                        XmlElement scale_y = document.CreateElement("y");
                        scale_y.InnerText = obj.transform.localScale.y + "";
                        XmlElement scale_z = document.CreateElement("z");
                        scale_z.InnerText = obj.transform.localScale.z + "";

                        scale.AppendChild(scale_x);
                        scale.AppendChild(scale_y);
                        scale.AppendChild(scale_z);

                        transform.AppendChild(position);
                        transform.AppendChild(rotation);
                        transform.AppendChild(scale);

                        gameObject.AppendChild(transform);
                        scenes.AppendChild(gameObject);
                        root.AppendChild(scenes);
                        document.AppendChild(root);
                        document.Save(path);
                    }
                }
            }
        }
        AssetDatabase.Refresh();
    }
}
#endregion
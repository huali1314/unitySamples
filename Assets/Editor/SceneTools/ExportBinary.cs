using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

class ExportBinary : Editor
{
	[MenuItem("CustomTools/SceneTools/ExportBinary")]
	static void ExportSceneBinary()
	{
		string filepath = Application.dataPath + @"/StreamingAssets/binary.txt";
		if (File.Exists(filepath))
		{
			File.Delete(filepath);
		}
		FileStream fs = new FileStream(filepath, FileMode.Create);
		BinaryWriter bw = new BinaryWriter(fs);
		foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
		{
			if (S.enabled)
			{
				string name = S.path;
				EditorSceneManager.OpenScene(name);

				foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
				{
					if (obj.transform.parent == null)
					{
						//注解 直接写入字符串
						bw.Write(name);
						bw.Write(obj.name);

						short posx = (short)(obj.transform.position.x * 100);
						bw.Write(posx);
						bw.Write((short)(obj.transform.position.y * 100.0f));
						bw.Write((short)(obj.transform.position.z * 100.0f));
						bw.Write((short)(obj.transform.rotation.eulerAngles.x * 100.0f));
						bw.Write((short)(obj.transform.rotation.eulerAngles.y * 100.0f));
						bw.Write((short)(obj.transform.rotation.eulerAngles.z * 100.0f));
						bw.Write((short)(obj.transform.localScale.x * 100.0f));
						bw.Write((short)(obj.transform.localScale.y * 100.0f));
						bw.Write((short)(obj.transform.localScale.z * 100.0f));

					}
				}

			}
		}

		bw.Flush();
		bw.Close();
		fs.Close();
	}

}

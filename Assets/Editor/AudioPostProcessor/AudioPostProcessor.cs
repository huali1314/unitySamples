using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public class AudioPostProcessor : AssetPostprocessor
{
    private string[] tempArray;
    private string directoryName;
    //音频导入之前调用
    public void OnPreprocessAudio()
    {
        AudioImporter audio = this.assetImporter as AudioImporter;
        assetPath = audio.assetPath;
        
        tempArray = assetPath.Split('/');
        directoryName = tempArray[tempArray.Length - 2];
        switch (directoryName)
        {
            case "BGM":
                break;
            case "Effect":
                break;
            case "UI":
                break;
            default:
                break;

        }
        //audio. = AudioCompressionFormat.MP3;
    }
    //音频导入之后调用
    public void OnPostprocessAudio(AudioClip clip)
    {
        Debug.Log(clip+ "=======OnPostprocessAudio========");
    }
}

﻿using UnityEditor.Build.Content;

namespace UnityEditor.Build.Pipeline.Tests
{
    internal static class ReflectionExtentions
    {
        public static void SetFileName(this ref ResourceFile file, string filename)
        {
#if UNITY_2019_3_OR_NEWER
            file.fileName = filename;
#else
            var fieldInfo = typeof(ResourceFile).GetField("m_FileName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object boxed = file;
            fieldInfo.SetValue(boxed, filename);
            file = (ResourceFile)boxed;
#endif
        }

        public static void SetFileAlias(this ref ResourceFile file, string fileAlias)
        {
#if UNITY_2019_3_OR_NEWER
            file.fileAlias = fileAlias;
#else
            var fieldInfo = typeof(ResourceFile).GetField("m_FileAlias", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object boxed = file;
            fieldInfo.SetValue(boxed, fileAlias);
            file = (ResourceFile)boxed;
#endif
        }

        public static void SetSerializedFile(this ref ResourceFile file, bool serializedFile)
        {
#if UNITY_2019_3_OR_NEWER
            file.serializedFile = serializedFile;
#else
            var fieldInfo = typeof(ResourceFile).GetField("m_SerializedFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object boxed = file;
            fieldInfo.SetValue(boxed, serializedFile);
            file = (ResourceFile)boxed;
#endif
        }

        public static void SetResourceFiles(this ref WriteResult result, ResourceFile[] resourceFiles)
        {
            var fieldInfo = typeof(WriteResult).GetField("m_ResourceFiles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object boxed = result;
            fieldInfo.SetValue(boxed, resourceFiles);
            result = (WriteResult)boxed;
        }

        public static void SetSerializedObjects(this ref WriteResult result, ObjectSerializedInfo[] osis)
        {
            var fieldInfo = typeof(WriteResult).GetField("m_SerializedObjects", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object boxed = result;
            fieldInfo.SetValue(boxed, osis);
            result = (WriteResult)boxed;
        }

        public static void SetHeader(this ref ObjectSerializedInfo osi, SerializedLocation serializedLocation)
        {
            var fieldInfo = typeof(ObjectSerializedInfo).GetField("m_Header", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object boxed = osi;
            fieldInfo.SetValue(boxed, serializedLocation);
            osi = (ObjectSerializedInfo)boxed;
        }

        public static void SetFileName(this ref SerializedLocation serializedLocation, string filename)
        {
            var fieldInfo = typeof(SerializedLocation).GetField("m_FileName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object boxed = serializedLocation;
            fieldInfo.SetValue(boxed, filename);
            serializedLocation = (SerializedLocation)boxed;
        }

        public static void SetOffset(this ref SerializedLocation serializedLocation, ulong offset)
        {
            var fieldInfo = typeof(SerializedLocation).GetField("m_Offset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object boxed = serializedLocation;
            fieldInfo.SetValue(boxed, offset);
            serializedLocation = (SerializedLocation)boxed;
        }
    }
}
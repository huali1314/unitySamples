using System.Collections;
using UnityEngine;
using System;
using System.IO;
public class ByteArray
{
    //创建内存流对象，并将缓存数据写进去
    MemoryStream ms = new MemoryStream();
    BinaryReader binaryReader;
    BinaryWriter binaryWriter;
    public ByteArray(){
        binaryReader = new BinaryReader(ms);
        binaryWriter = new BinaryWriter(ms);
    }
    public ByteArray(byte[] buff){
        ms = new MemoryStream(buff);
        binaryReader = new BinaryReader(ms);
        binaryWriter = new BinaryWriter(ms);
    }
    public  void Close(){
        ms.Close();
        binaryWriter.Close();
        binaryReader.Close();
    }
    public bool Readble{
        get{return ms.Length >ms.Position;}
    }
    public int Length{
        get{return (int)ms.Length;}
    }
    public int Position{
        get{return (int)ms.Position;}
    }
    public void Read(out int value){
        value = binaryReader.ReadInt32();
    }
    public void Read(out byte value){
        value = binaryReader.ReadByte();
    }
    public void Read(out bool value){
        value = binaryReader.ReadBoolean();
    }
    public void Read(out string value){
        value = binaryReader.ReadString();
    }
    public void Read(out byte[] value,int length){
        value = binaryReader.ReadBytes(length);
    }
    public void Read(out double value){
        value = binaryReader.ReadDouble();
    }
    public void Read(out float value){
        value = binaryReader.ReadSingle();
    }
    public void Read(out long value){
        value = binaryReader.ReadInt64();
    }
    public void Write(int value){
        binaryWriter.Write(value);
    }
    public void Write(byte value){
        binaryWriter.Write(value);
    }
    public void Write(byte[] value){
        binaryWriter.Write(value);
    }
    public void Write(bool value){
        binaryWriter.Write(value);
    }
    public void Write(float value){
        binaryWriter.Write(value);
    }
    public void Write(long value){
        binaryWriter.Write(value);
    }
    public void Write(double value){
        binaryWriter.Write(value);
    }
    public void Write(string value){
        binaryWriter.Write(value);
    }
    public void reposition(){
        ms.Position = 0;
    }
    public byte[] GetBuffer(){
        byte[] result = new byte[ms.Length];
        Buffer.BlockCopy(ms.GetBuffer(),0,result,0,(int)ms.Length);
        return result;
    }
}

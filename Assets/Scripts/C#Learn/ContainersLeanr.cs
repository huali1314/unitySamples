using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainersLeanr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ArrayTest();
        // ArrayListTest();
        // ListTest();
        // DictionaryTest();
        // StackTest();
        QueueTest();
    }
    //数组
    //    优点：内存分配连续，检索速度快，修改数据简单
    //    缺点：使用时必须指定内存大小，在数组中插入数据比较困难，空间分配过大会造成内存浪费，内存过小加上使用不当会造成内存溢出
    //    提示：提前不知道大小的情况下不建议使用数组

    public void ArrayTest(){
        string [] str = new string[3];
        str[0] = "a";
        str[1] = "b";
        str[2] = "c";
        UnityEngine.Debug.Log("=========array====");
        for(int i = 0;i < str.Length;i++){
            UnityEngine.Debug.Log("=====str====" + i + "=" + str[i] + "/t");
        }
        str[2] = "d";
        for(int i = 0;i < str.Length;i++){
            UnityEngine.Debug.Log("=====str====" + i + "=" + str[i] + "/t");
        }
    }
    //数组列表(ArrayList)：.Net Framework中提供的一种用于检索和存储的专用类,在System.Collections命名空间下，使用时不需要指定大小，能根据存储数据的大小自动分配内存
    //    优点：不需要指定大小，继承了IList，对于增删改查很方便，ArrayList中能够存储不同类型的数据，在内存中以object的类型存储。
    //    缺点：由于所有类型数据最终都以object类型存储，所以数据类型是不安全的。再者即便存储的是相同类型的数据，存储都以object类型存储，这样的使用的时候就会存在类型转换，存在装箱拆箱操作，耗费性能

    public void ArrayListTest(){
        ArrayList al = new ArrayList();
        al.Add("==========");
        al.Add("aaa");
        al.Add("bbb");
        for(int i = 0;i< al.Count;i++){
            UnityEngine.Debug.Log("====al===" + i + "=" + al[i]);
        }
        al.RemoveAt(1);
        for(int i = 0;i< al.Count;i++){
            UnityEngine.Debug.Log("====all===" + i + "=" + al[i]);
        }
    }
    //列表（List<T>）:定义是可以指定存储的数据类型，将类型安全的人物交给我编译器，代码编译的时候，编辑器会强制使用正确的数据类型，减少了强制类型的使用。（类型转换会有装箱拆箱的操作）
    public void ListTest(){
        List<string> ls = new List<string>();
        ls.Add("aaaa");
        ls.Add("111");
        ls.Add("222");
        for(int i = 0;i< ls.Count;i++){
            UnityEngine.Debug.Log("====ls===" + i + "=" + ls[i]);
        }
        ls.RemoveAt(0);//调用RemoveAt函数移除第一条数据，后面的数据会自动向前补齐
        for(int i = 0;i< ls.Count;i++){
            UnityEngine.Debug.Log("====lss===" + i + "=" + ls[i]);
        }
    }
    //字典（Dictionary<key,value>):键值对存储方式，由于多了key的数据，使用上内存会相较于前几种大，但检索速度会更快。性能上比较一般，初始化时也必须指定类型
    public void DictionaryTest(){
        Dictionary<int,string> d = new Dictionary<int, string>();
        d.Add(1,"aaa");
        d.Add(2,"bbb");
        d.Add(3,"ccc");
        foreach(KeyValuePair<int,string> pair in d){
            UnityEngine.Debug.Log("====Key===" + pair.Key.ToString() + "==Value===" + pair.Value.ToString());
        }
        d.Remove(2);
        foreach(KeyValuePair<int,string> pair in d){
            UnityEngine.Debug.Log("====Key===" + pair.Key.ToString() + "==Value===" + pair.Value.ToString());
        }
    }
    //栈（Stack<T>）：先进后出
    public void StackTest(){
        Stack<int> s = new Stack<int>();
        s.Push(100);
        s.Push(101);
        s.Push(102);
        s.Push(103);
        UnityEngine.Debug.Log("======count=====" + s.Count);
        // for(int i = 0;i < s.Count;i++){
            UnityEngine.Debug.Log("===========" + s.Pop());
            UnityEngine.Debug.Log("===========" + s.Pop());
            UnityEngine.Debug.Log("===========" + s.Pop());
            UnityEngine.Debug.Log("===========" + s.Pop());
        // }
        UnityEngine.Debug.Log("======count1=====" + s.Count);
        s.Push(104);
        s.Peek();
        UnityEngine.Debug.Log("======count2=====" + s.Count);
    }
    //队列（Queue<T>）；先进先出
    public  void QueueTest(){
        Queue<int> q = new Queue<int>();
        q.Enqueue(111);//向队尾插入一个元素
        q.Enqueue(222);
        q.Enqueue(333);
        UnityEngine.Debug.Log("==========" + q.Dequeue());
        UnityEngine.Debug.Log("==count========" + q.Count);
        UnityEngine.Debug.Log("==cotains========" + q.Contains(111));
        UnityEngine.Debug.Log("==cotains========" + q.Contains(222));
        int []inaa = q.ToArray();
        UnityEngine.Debug.Log("==111========" + q.Count);
        UnityEngine.Debug.Log("==222========" + inaa.Length);
        q.Clear();
    }
    //双端队列（Deque<T>）：灵活操作
    // public  void DequeTest(){
        // Deque<string> d = new Deque<string>();
        
    // }

}

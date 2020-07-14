using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Delagate
{
    //代理：至少0个参数，至多32个参数，可以无返回值，可以指定返回值类型
    public delegate void delegate1();
    delegate1 callback = new delegate1(Func1);
    public delegate void delegate2(int p1);
    delegate2 callback1 = new delegate2(Func1);

    public delegate int delegate3(int p);
    public event delegate3 callback2;
    public void EventTest() {
        int a = 1;
        int r = callback2(a);
    }
    //Action:至少0个参数，至多16个参数，无返回值
    //无传入参数
    public void ActionTest(Action action) {

    }
    //有一个传入参数
    public void ActionTest<T>(Action<T> action) {

    }

    //Func:至少0个参数，至多16个参数,必须有返回值
    //无参数，int型返回值
    public void FuncTest(Func<int> func) {

    }
    //一个object参数，string类型返回值
    public void FuncTest1(Func<object, string> func) {
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }
    public static void Func1()
    {

    }
    public static void Func1(int p1)
    {
        
    }
    public static void Func1(string p1)
    {

    }
    public static void Func1(object p1)
    {

    }
}
public class MainClass
{
    public static void Main()
    {
        Delagate d = new Delagate();
        d.callback2 += DelegateFunc;
        d.callback2 += DelegateFunc1;
    }
    public static int DelegateFunc(int p)
    {
        return 1 + p;
    }
    public static int DelegateFunc1(int p)
    {
        return 1 + p;
    }
}

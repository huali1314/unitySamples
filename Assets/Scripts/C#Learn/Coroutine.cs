using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
public class Coroutine:MonoBehaviour
{
    private static int loopCount = 0;
    private static SynchronizationContext synchronizationContext;
    private static void Start()
    {
        CoroutineTest();
    }
    private static void CoroutineTest()
    {
        synchronizationContext = new SynchronizationContext();
        WaitTimeAsync(5000, WaitTimeFinishCallback);
        //while (true)
        //{
        //    Thread.Sleep(1);
        //    ++loopCount;
        //    if (loopCount % 10000 == 0)
        //    {
        //        Console.WriteLine("loop count:" + loopCount);
        //    }
        //}
    }

    private static void WaitTimeAsync(int v, Action waitTimeFinishCallback)
    {
        Thread thread = new Thread(() => WaitTime(v, waitTimeFinishCallback));
        thread.Start();
    }
    private static void WaitTimeFinishCallback()
    {
        Debug.Log("WaitTimeAsync finish loopCount:" + loopCount);
        //Console.WriteLine("WaitTimeAsync finish loopCount:" + loopCount);
    }
    private static void WaitTime(int waitTIme, Action action)
    {
        Thread.Sleep(waitTIme);
        synchronizationContext.Send((o) => action(), null);
        //SynchronizationContext.Current.Post((o) => action(), null);
    }
}

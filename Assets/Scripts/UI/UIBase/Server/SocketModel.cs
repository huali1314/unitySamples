using UnityEngine;
using System.Collections;
public class SocketModel{
    //一级协议，用于区分所属模块
    public byte type{get;set;}
    //二级协议，用于区分模块下的子模块
    public int area{set;get;}
    //三级协议,用于区分当前处理的逻辑
    public int command{set;get;}
    //消息体，当前需要处理的主体数据
    public object message{set;get;}

    public SocketModel(){

    }
    public SocketModel(byte t,int a,int c,object m){
        this.type = t;
        this.area = a;
        this.command = c;
        this.message = m;
    }
    public T GetMessage<T>(){
        return(T)message;
    }
}

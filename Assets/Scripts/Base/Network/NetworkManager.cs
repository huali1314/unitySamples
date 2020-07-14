using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
public class Message
{
    private int m_MsgLength;
    private Int32 m_ProtocalID;
    public Message(int length)
    {
        m_MsgLength = length;
    }
    public void setProtocalID(Int32 id)
    {
        m_ProtocalID = id;
    }
    public void Add(byte[] b, int index, int length)
    {

    }
}
public class NetworkManager : Singleton<NetworkManager>
{
    public enum ServerType {
        GateServer = 0,
        BalanceServer,
        LohinServer
    }
    //tcp客户端
    private TcpClient m_Client = null;
    private TcpClient m_Connecting = null;
    private string m_IP = "127.0.0.1";
    private Int32 m_Port = 40001;
    private Int32 m_n32ConnectTimes = 0;
    private ServerType serverType = ServerType.BalanceServer;

    //连接时间
    private float m_CanConnectTime = 0.0f;
    //接收超时时间
    private float m_RecvOverTime = 0.0f;
    private float m_RecvOverDelayTime = 6000f;
    private Int32 m_ConnectOverCount = 0;
    private float m_ConnectOverTime = 0.0f;
    private Int32 m_RecvOverCount = 0;

    public bool canReconnect = false;
    public byte[] m_RecvBuffer = new byte[2 * 1024 * 1024];//分配2m的接收缓冲区
    public Int32 m_RecvPos = 0;
    public bool mPause = false;

    IAsyncResult mRecvResult;
    IAsyncResult mConnectResult;

    private bool m_bCustomHandleMessage;
    //发送数据stream
    public System.IO.MemoryStream mSendStream = new System.IO.MemoryStream();
    //接收数据stream
    public List<int> mReceiveMsgIDs = new List<int>();

    public List<System.IO.MemoryStream> mReceiveStreams = new List<System.IO.MemoryStream>();
    public List<System.IO.MemoryStream> mReceiveStreamsPool = new List<System.IO.MemoryStream>();

    public NetworkManager()
    {
        m_bCustomHandleMessage = false;
        for(int i = 0;i< 50; ++i)
        {
            mReceiveStreamsPool.Add(new System.IO.MemoryStream());
        }
    }
    public void Pause()
    {
        mPause = true;
    }
    public void Resume()
    {
        mPause = false;
    }

    ~NetworkManager()
    {
        foreach (System.IO.MemoryStream one in mReceiveStreams)
        {
            one.Close();
        }
        foreach (System.IO.MemoryStream one in mReceiveStreamsPool)
        {
            one.Close();
        }
        //发送stream
        if (mSendStream != null)
        {
            mSendStream.Close();
        }
        if (m_Client != null)
        {
            m_Client.Client.Shutdown(SocketShutdown.Both);
            m_Client.GetStream().Close();
            m_Client.Close();
            m_Client = null;
        }
    }
    //初始化NetworkManager，获取地址和端口
    public void Init(string host, Int32 port, ServerType type, bool customHandleMessage = false)
    {
        m_bCustomHandleMessage = customHandleMessage;
        UnityEngine.Debug.Log("set network ip:" + host + "port:" + port + "type:" + type);
        m_IP = host;
        m_Port = port;
        serverType = type;
        m_n32ConnectTimes = 0;
        canReconnect = true;
        m_RecvPos = 0;
#if UNITY_EDITOR
        m_RecvOverDelayTime = 20000f;
#endif
    }
    public void UnInit()
    {
        canReconnect = false;
    }
    //连接服务器，在update中调用
    public void Connect() {
        if (!canReconnect) return;
        if (m_CanConnectTime > Time.time) return;
        if (m_Client != null)
        {
            throw new Exception("The socket is connecting,cannot connect again!");
        }
        if (m_Connecting != null)
        {
            throw new Exception("The socket is connecting,cannot connect again!");
        }
        try
        {
            m_Connecting = new TcpClient();
            mConnectResult = m_Connecting.BeginConnect(m_IP, m_Port, null, m_Connecting);
            m_ConnectOverCount = 0;
            m_ConnectOverTime = Time.time + 2;
        }
        catch(Exception exc)
        {
            UnityEngine.Debug.Log("error:" + exc.ToString());
            m_Client = m_Connecting;
            m_Connecting = null;
            mConnectResult = null;
            OnConnectError(m_Client, null);
        }
    }

    private void OnConnectError(TcpClient m_Client, object p)
    {
        UnityEngine.Debug.Log("OnConnectError begin");
        try
        {
            m_Client.Client.Shutdown(SocketShutdown.Both);
            m_Client.GetStream().Close();
            m_Client.Close();
            m_Client = null;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }
        mRecvResult = null;
        m_Client = null;
        m_RecvPos = 0;
        m_RecvOverCount = 0;
        m_ConnectOverCount = 0;

        EventCenter.Broadcast((Int32)GameEventEnum.ConnectServerFail);
        UnityEngine.Debug.Log("OnConnection End");
    }

    public void Close()
    {
        if (m_Client != null)
        {
            OnClose(m_Client, null);
        }
    }

    private void OnClose(TcpClient m_Client, object p)
    {
        EventCenter.Broadcast((Int32)GameEventEnum.ConnectServerFail);
        try
        {
            m_Client.Client.Shutdown(SocketShutdown.Both);
            m_Client.GetStream().Close();
            m_Client.Close();
            m_Client = null;
        }
        catch (Exception exc)
        {
            UnityEngine.Debug.Log(exc.ToString());
        }
        mRecvResult = null;
        m_Client = null;
        m_RecvPos = 0;
        m_RecvOverCount = 0;
        m_ConnectOverCount = 0;
        mReceiveStreams.Clear();
        mReceiveMsgIDs.Clear();
    }

    public void Update(float fDeltaTime)
    {
        //如果已经有客户端连接
        if (m_Client != null)
        {
            if (mPause) return;
            //处理mRecviveStreams中缓存的消息
            DealWithMsg();
            if (mRecvResult != null)
            {
                //接收数据超过200并且超时，关闭连接
                if (m_RecvOverCount > 200 && Time.time > m_RecvOverTime)
                {
                    UnityEngine.Debug.Log("recv data over 200,so close network");
                    Close();
                    return;
                }
                ++m_RecvOverCount;//记录接收次数
                if (mRecvResult.IsCompleted)//读取数据结束
                {
                    try
                    {
                        Int32 n32BytesRead = m_Client.GetStream().EndRead(mRecvResult);
                        m_RecvPos += n32BytesRead;
                        if (n32BytesRead == 0)
                        {
                            UnityEngine.Debug.Log("recv data over 200,so close network2");
                            Close();
                            return;
                        }
                    }
                    catch (Exception exc)
                    {
                        UnityEngine.Debug.Log(exc.ToString());
                        Close();
                        return;
                    }
                    OnDataReceived(null, null);//接收数据处理，往stream中填充网络数据
                    if (m_Client != null)
                    {
                        try
                        {
                            mRecvResult = m_Client.GetStream().BeginRead(m_RecvBuffer, m_RecvPos, m_RecvBuffer.Length - m_RecvPos, null, null);//开始读取
                            m_RecvOverTime = Time.time + m_RecvOverDelayTime;
                            m_RecvOverCount = 0;
                        }
                        catch (Exception exc)
                        {
                            UnityEngine.Debug.Log(exc.ToString());
                            Close();
                            return;
                        }
                    }
                }
            }
            if (m_Client != null && m_Client.Connected == false)
            {
                UnityEngine.Debug.Log("Client is close by stream,so close it now");
                Close();
                return;
            }
        }
        //如果客户端正在连接
        else if (m_Connecting != null)
        {
            //如果连接次数超过200次并且连接超时，连接失败，断开连接
            if (m_ConnectOverCount > 200 && Time.time > m_ConnectOverTime)
            {
                UnityEngine.Debug.Log("Can't connect ,so close it network");
                m_Client = m_Connecting;
                m_Connecting = null;
                mConnectResult = null;
                OnConnectError(m_Client, null);
                return;
            }
            ++m_ConnectOverCount;//记录连接次数
            if (mConnectResult.IsCompleted)//连接参数设置
            {
                m_Client = m_Connecting;
                m_Connecting = null;
                mConnectResult = null;
                if (m_Client.Connected)
                {
                    try
                    {
                        m_Client.NoDelay = true;
                        m_Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 2000);
                        m_Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2000);
                        m_Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                        mRecvResult = m_Client.GetStream().BeginRead(m_RecvBuffer, 0, m_RecvBuffer.Length, null, null);//从网络缓冲区异步读取数据
                        m_RecvOverTime = Time.time + m_RecvOverTime;
                        m_RecvOverCount = 0;
                        OnConnected(m_Client, null);
                    }
                    catch (Exception exc)
                    {
                        UnityEngine.Debug.Log(exc.ToString());
                        Close();
                        return;
                    }
                }
                else
                {
                    OnConnectError(m_Client, null);
                }
            }

        }
        //如果没有连接
        else
        {
            Connect();
        }
    }

    private void OnConnected(object  sender, EventArgs e)
    {
        switch (serverType)
        {
            case ServerType.BalanceServer:
                {
                    
                }
                break;
            case ServerType.GateServer:
                {
                    ++m_n32ConnectTimes;
                    if (m_n32ConnectTimes > 1)
                    {

                    }
                    else
                    {

                    }
                    EventCenter.Broadcast((Int32)GameEventEnum.ConnectServerSuccess);
                    break;
                }
            case ServerType.LohinServer:
                {

                }
                break;
        }
    }

    private void OnDataReceived(object p1, object p2)
    {
        throw new NotImplementedException();
    }

    private void DealWithMsg()
    {
        while (mReceiveMsgIDs.Count > 0 && mReceiveStreams.Count > 0)
        {
            int type = mReceiveMsgIDs[0];
            System.IO.MemoryStream iostream = mReceiveStreams[0];
            mReceiveMsgIDs.RemoveAt(0);
            mReceiveStreams.RemoveAt(0);
            try
            {
                if (m_bCustomHandleMessage)
                {
                    EventCenter.Broadcast<System.IO.Stream, int>((Int32)GameEventEnum.GameEvent_NotifyNetMessage, iostream, type);
                }
                else
                {

                }
                //通知收到了网络消息
                if (mReceiveStreamsPool.Count < 100)
                {
                    mReceiveStreamsPool.Add(iostream);
                }
                else
                {
                    iostream = null;
                }
            }
            catch (Exception exc)
            {
                UnityEngine.Debug.Log(exc.ToString());
            }
        }
    }

    //发送消息向服务器，两个参数，第一个消息体，第二个消息id
    public void SendMsg(object pMsg, Int32 n32MsgID)
    {
        if (m_Client != null)
        {
            //清除stream
            mSendStream.SetLength(0);
            mSendStream.Position = 0;
            Message pcMsg = new Message((int)mSendStream.Length);
            
        }
    }
}

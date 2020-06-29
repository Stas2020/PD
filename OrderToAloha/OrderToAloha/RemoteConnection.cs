using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;



namespace OrderToAloha
{
    class RemoteConnection
    {

        public TcpClient mClient;
        public TcpListener mListener;
        public Thread mListenerThread;
        int port = 0;
        string ServerName = "";


        internal RemoteConnection()
        {


        }

        internal delegate void ConnectionAccertDelegate(TcpClient Client);

        internal event ConnectionAccertDelegate ConnectionAccert;

        // Invoke the Changed event; called whenever list changes:
        protected virtual void OnConnectionAccert(TcpClient Client)
        {
            if (ConnectionAccert != null)
                ConnectionAccert(Client);
        }


        internal delegate void DataReciveDelegate(object Fi);

        internal event DataReciveDelegate DataRecive;

        // Invoke the Changed event; called whenever list changes:
        protected virtual void OnDataRecive(object Fi)
        {
            if (DataRecive != null)
                DataRecive(Fi);
        }




        internal void StartServerLisenter(int mPort)
        {
            //mListenerThread = new Thread(DoListen);
            port = mPort;
            DoListen();

        }
        internal void StopServerLisenter()
        {
            mListener.Stop();
        }

        private void AcceptCallback(IAsyncResult result)
        {
            try
            {
                TcpListener mTcpListener = (TcpListener)result.AsyncState;
                //    Socket  mSocet = mTcpListener.EndAcceptSocket(result);
                //  TcpClient mClient =  mListener.AcceptTcpClient();
                //     byte[] Bufer = new byte [2048];
                ConnectionInfo connection = new ConnectionInfo();
                connection.Socket = mTcpListener.EndAcceptSocket(result);
                connection.Buffer = new byte[10240];


                connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), connection);



                //  DoListen();

                mTcpListener.BeginAcceptSocket(new
                        AsyncCallback(AcceptCallback), mTcpListener);

                OnConnectionAccert(mClient);
            }
            catch
            { }
        }

        internal object getObjectWithByteArray(byte[] theByteArray)
        {

            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;
            return bf1.Deserialize(ms);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection =
              (ConnectionInfo)result.AsyncState;

            int bytesRead =
                connection.Socket.EndReceive(result);

            if (bytesRead > 0)
            {
                OnDataRecive(getObjectWithByteArray(connection.Buffer));
            }

        }

        private void DoListen()
        {
            try
            {
                IPHostEntry localMachineInfo = Dns.GetHostEntry(Dns.GetHostName());
                mListener = new TcpListener(IPAddress.Any, port);
                mListener.Start();



                mListener.BeginAcceptSocket(new
                       AsyncCallback(AcceptCallback), mListener);


            }
            catch (Exception e)
            {

                string s = e.Message;
            }

        }

        internal static byte[] getByteArrayWithObject(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }

        static internal bool SendData(string ServerName, int port, object Fi)
        {
               Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                s.Connect(ServerName,port);

                s.Send(getByteArrayWithObject(Fi));

                s.Disconnect(false);

                s.Close();

                Utils.ToLog("SendData Ok");
                return true;
            
        }


        static internal bool SendData(IPAddress ServerIP, int port, object Fi)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            s.Connect(ServerIP, port);

            s.Send(getByteArrayWithObject(Fi));

            s.Disconnect(false);

            s.Close();

            Utils.ToLog("SendData Ok");
            return true;

        }




    }
    internal class ConnectionInfo
    {
        public Socket Socket;
        public byte[] Buffer;
    }
  
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms; 


namespace PDiscountCard.RemoteCommands
{
    class RemoteConnection
    {

        public TcpClient mClient;
        public TcpListener mListener;
        public Thread mListenerThread;
        int port = 0;
        string ServerName = "";


        public RemoteConnection()
        {


        }

        public delegate void ConnectionAccertDelegate(TcpClient Client);

        public event ConnectionAccertDelegate ConnectionAccert;

        // Invoke the Changed event; called whenever list changes:
        protected virtual void OnConnectionAccert(TcpClient Client)
        {
            if (ConnectionAccert != null)
                ConnectionAccert(Client);
        }


        public delegate void DataReciveDelegate(object  Fi);

        public event DataReciveDelegate DataRecive;

        // Invoke the Changed event; called whenever list changes:
        protected virtual void OnDataRecive(object Fi)
        {
            if (DataRecive != null)
                DataRecive(Fi);
        }




        public void StartServerLisenter(int mPort)
        {
            //mListenerThread = new Thread(DoListen);
            port = mPort;
            DoListen();

        }
        public void StopServerLisenter()
        {
            mListener.Stop();
        }

        private void AcceptCallback(IAsyncResult result)
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

        public object getObjectWithByteArray(byte[] theByteArray)
        {

            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;
            return bf1.Deserialize(ms);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
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
            catch(Exception e)
            {
                Utils.ToCardLog("Error ReceiveCallback " + e.Message);
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

        public static byte[] getByteArrayWithObject(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }

        static public bool SendData(string ServerName, int port, object  Fi, bool TryConnect)
        {
            try
            {

                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                s.Connect(ServerName,port);

                s.Send(getByteArrayWithObject(Fi));

                s.Disconnect(false);

                s.Close();


                Utils.ToLog("SendData Ok");
                return true;
            }
            catch (Exception e)
            {
                if (!TryConnect)
                {
                //    MessageBox.Show("Ошибка отправки команды на компьютер: " + ServerName + " порт: " + port + Environment.NewLine + e.Message);
                }
                Utils.ToLog("SendData Error " + e.Message);
                return false;
            }
        }

        public bool SendData(string ServerName, int port, object Fi)
        {
            return SendData(ServerName, port, Fi, false);

        }


    }
  
}

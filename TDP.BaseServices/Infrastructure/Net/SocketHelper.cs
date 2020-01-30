//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//  NOTE: Part of this code is taken from the book "C# Network Programming"
//        by Richard Blum, published in 2003
//
//*****************************************************************************

using System;
using System.Net.Sockets;
using TDP.BaseServices.Infrastructure.Serialization;

namespace TDP.BaseServices.Infrastructure.Net
{
    public class SocketHelper
    {
        private SocketHelper()
        {

        }

        private static int SendVarData(Socket socket, byte[] data)
        {
            int Total = 0;
            int Size = data.Length;
            int Dataleft = Size;
            int Sent;
            byte[] Datasize = new byte[4];
            Datasize = BitConverter.GetBytes(Size);
            Sent = socket.Send(Datasize);
            while (Total < Size)
            {
                Sent = socket.Send(data, Total, Dataleft, SocketFlags.None);
                Total += Sent;
                Dataleft -= Sent;
            }
            return Total;
        }

        private static byte[] ReceiveVarData(Socket socket)
        {
            int Total = 0;
            int Recv;
            byte[] Datasize = new byte[4];
            Recv = socket.Receive(Datasize, 0, 4, 0);
            int Size = BitConverter.ToInt32(Datasize, 0);
            int Dataleft = Size;
            byte[] Data = new byte[Size];
            while (Total < Size)
            {
                Recv = socket.Receive(Data, Total, Dataleft, 0);
                Total += Recv;
                Dataleft -= Recv;
            }
            return Data;
        }

        public static void SendObject<T>(Socket socket, T objectToSend)
        {
            byte[] objectSerialized = SerializationHelper.Serialize(objectToSend);
            SendVarData(socket, objectSerialized);
        }

        public static T ReceiveObject<T>(Socket socket)
        {
            byte[] ObjectSerialized = ReceiveVarData(socket);

            if (ObjectSerialized.Length == 0)
                return default(T);
               
            return SerializationHelper.Deserialize<T>(ObjectSerialized);
        }

        public static object ReceiveObject(Socket socket)
        {
            byte[] ObjectSerialized = ReceiveVarData(socket);

            if (ObjectSerialized.Length == 0)
                return null;

            return SerializationHelper.Deserialize(ObjectSerialized);
        }
    }
}

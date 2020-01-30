//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using TDP.BaseServices.Infrastructure.Net;
using TDP.PushLib.Messages.Core;

namespace TDP.PushLib.Server
{
    class Client<T> : IDisposable
    {
        private ManualResetEvent _BlockSendPushMessage;
        private Socket _ClientSocket;
        private ConcurrentQueue<PushMessage> _PendingMessages;
        private bool _DisconnectRequested;

        public T ID { get; set; }

        public Client(Socket clientSocket)
        {
            _PendingMessages = new ConcurrentQueue<PushMessage>();
            _BlockSendPushMessage = new ManualResetEvent(false);
            _ClientSocket = clientSocket;
        }
                
        public void SendPushMessage(PushMessage pushMessage)
        {
            _PendingMessages.Enqueue(pushMessage);
            _BlockSendPushMessage.Set();
        }

        public bool ManagePushMessages()
        {
            PushMessage message;
            _BlockSendPushMessage.WaitOne();

            if (_DisconnectRequested)
                return false;

            while (_PendingMessages.TryDequeue(out message))
            {
                SendObject(message);
            }

            if (_PendingMessages.Count == 0 && !_DisconnectRequested)
                _BlockSendPushMessage.Reset();

            return true;
        }

        public void Disconnect()
        {
            _DisconnectRequested = true;
            _BlockSendPushMessage.Set();
        }

        public void SendObject<O>(O objectToSend)
        {
            SocketHelper.SendObject(_ClientSocket, objectToSend);
        }

        public O ReceiveObject<O>()
        {
            return SocketHelper.ReceiveObject<O>(_ClientSocket);
        }

        public object ReceiveObject()
        {
            return SocketHelper.ReceiveObject(_ClientSocket);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void DisposeConnection()
        {
            try
            {
                if (_ClientSocket.Connected)
                    _ClientSocket.Disconnect(true); // TODO: Check the parameter 'reuse socket'
                _ClientSocket.Close();
                _ClientSocket.Dispose();
            }
            catch { }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                DisposeConnection();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DBAccess() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

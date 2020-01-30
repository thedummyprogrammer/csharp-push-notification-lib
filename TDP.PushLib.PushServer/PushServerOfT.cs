//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TDP.BaseServices.Infrastructure.Configuration;
using TDP.PushLib.Messages;
using TDP.PushLib.Messages.Core;
using TDP.PushLib.Server.Infrastructure;
using TDP.PushLib.Server.Infrastructure.Configuration;

namespace TDP.PushLib.Server
{
    public class PushServer<T>
    {
        #region "Event definitions"
        public event EventHandler InitStarted;
        protected virtual void OnInitStarted(EventArgs e)
        {
            EventHandler handler = InitStarted;
            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = singleCast.Target as ISynchronizeInvoke;
                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        syncInvoke.Invoke(singleCast, new object[] { this, e });
                    else
                        singleCast(this, e);
                }
            }
        }

        public delegate void InitEndedDelegate(object sender, InitEndedEventArgs e);
        public event InitEndedDelegate InitEnded;
        protected virtual void OnInitEnded(InitEndedEventArgs e)
        {
            InitEndedDelegate handler = InitEnded;
            if (handler != null)
            {
                foreach (InitEndedDelegate singleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = singleCast.Target as ISynchronizeInvoke;
                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        syncInvoke.Invoke(singleCast, new object[] { this, e });
                    else
                        singleCast(this, e);
                }
            }
        }

        public event EventHandler ShutdownStarted;
        protected virtual void OnShutdownStarted(EventArgs e)
        {
            EventHandler handler = ShutdownStarted;
            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = singleCast.Target as ISynchronizeInvoke;
                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        syncInvoke.Invoke(singleCast, new object[] { this, e });
                    else
                        singleCast(this, e);
                }
            }
        }

        public event EventHandler ShutdownEnded;
        protected virtual void OnShutdownEnded(EventArgs e)
        {
            EventHandler handler = ShutdownEnded;
            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = singleCast.Target as ISynchronizeInvoke;
                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        syncInvoke.Invoke(singleCast, new object[] { this, e });
                    else
                        singleCast(this, e);
                }
            }
        }
        #endregion

        private ConfigReader _ConfigReader = new ConfigReader();
        private bool _ServerRunning;
        private Socket _SocketServer;
        private List<Client<T>> _ConnectedClients = new List<Client<T>>();
        private object _LockObject = new object();

        public async void StartAsync()
        {
            await Task.Run(() =>
                {
                    OnInitStarted(new EventArgs());

                    try
                    {
                        Stop();

                        _SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPAddress MyIPAddress = IPAddress.Parse(_ConfigReader.Get(ConfigReaderKeys.KeyListenIPAddress, Constants.DefaultListenIPAddress));
                        IPEndPoint MyEndPoint = new IPEndPoint(MyIPAddress, _ConfigReader.Get(ConfigReaderKeys.KeyListenPort, Constants.DefaultListenPort));
                        _SocketServer.Bind(MyEndPoint);
                        _SocketServer.Listen(_ConfigReader.Get(ConfigReaderKeys.KeyListenBacklog, Constants.DefaultListenBacklog));

                        _ServerRunning = true;
                    }
                    catch
                    {
                        OnInitEnded(new InitEndedEventArgs(new Result(-1, false, "Initialization failed")));
                        return;
                    }

                    WaitForClientsAsync();
                    
                    OnInitEnded(new InitEndedEventArgs(new Result(0, true, string.Empty)));
                }
            );
        }

        public void Stop()
        {
            if (_SocketServer != null)
            {
                if (_SocketServer.Connected)
                    _SocketServer.Disconnect(true);

                _SocketServer.Dispose();
            }

            lock (_LockObject)
            {
                foreach (Client<T> client in _ConnectedClients)
                {
                    client.Disconnect();
                }

                _ConnectedClients.Clear();
            }

            _ServerRunning = false;
        }

        public async void StopAsync()
        {
            await Task.Run(() =>
                {
                    OnShutdownStarted(new EventArgs());

                    Stop();

                    OnShutdownEnded(new EventArgs());
                }
            );
        }

        public void SendPushMessage(T clientID, string message)
        {
            //TODO: Need to sync here?
            PushMessage PushMessage = new PushMessage() { Message = message };
            foreach (Client<T> client in _ConnectedClients.Where(client => client.ID.Equals(clientID)))
            {
                client.SendPushMessage(PushMessage);
            }
        }

        private bool ExecRegister(Client<T> client, RegisterClient<T> message)
        {
            RegisterClientResponse Response = new RegisterClientResponse();
            
            bool ValidateAuthToken = _ConfigReader.Get(ConfigReaderKeys.KeyValidateAuthenticationToken, false);
            if (ValidateAuthToken)
            {
                string RSAKey = _ConfigReader.Get(ConfigReaderKeys.KeyValidateAuthenticationToken, string.Empty);
                int TokenDuration = _ConfigReader.Get(ConfigReaderKeys.KeyAuthenticationTokenDurationMinutes, Constants.AuthenticationTokenDurationMinutes);

                if (!message.AuthToken.IsValid(RSAKey, TokenDuration))
                {
                    Response.Success = false;
                    Response.ErrorCode = -1;
                    Response.Message = "Invalid authentication token.";

                    client.SendObject(Response);
                    return false;
                }
            }
            
            client.ID = message.AuthToken.ClientID;

            Response.Success = true;
            client.SendObject(Response);

            return true;
        }

        private async void ManageClientAsync(Socket clientSocket)
        {
            await Task.Run(() =>
            {
                using (Client<T> ThisClient = new Client<T>(clientSocket))
                {
                    try
                    {
                        object Message = ThisClient.ReceiveObject();

                        bool Result = false;
                        if (Message is RegisterClient<T>)
                        {
                            Result = ExecRegister(ThisClient, (RegisterClient<T>)Message);
                        }
                        else
                        {
                            // Couldn't build the RegisterClient message, exit
                            System.Diagnostics.Trace.WriteLine("Couldn't build the RegisterClient message, exit from ManageClientAsync");
                            return;
                        }

                        // Enter the push notification loop
                        if (Result)
                        {
                            lock (_LockObject)
                            {
                                _ConnectedClients.Add(ThisClient);
                            }
                            
                            bool RunClient = true;
                            while (RunClient)
                            {
                                RunClient = ThisClient.ManagePushMessages();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Track the error and remove the client
                        System.Diagnostics.Trace.WriteLine(ex.Message);

                        lock (_LockObject)
                        {
                            _ConnectedClients.Remove(ThisClient);
                        }
                    }
                }
            });
        }

        private async void WaitForClientsAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    while (_ServerRunning)
                    {
                        Socket ClientSocket = _SocketServer.Accept();

                        ManageClientAsync(ClientSocket);
                    }
                }
                catch (ObjectDisposedException)
                {
                    System.Diagnostics.Trace.WriteLine("The socket has been closed.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
            }
           );
        }
    }
}

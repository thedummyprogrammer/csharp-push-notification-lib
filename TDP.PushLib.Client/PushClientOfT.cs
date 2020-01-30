//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TDP.BaseServices.Infrastructure.Configuration;
using TDP.BaseServices.Infrastructure.Net;
using TDP.PushLib.Client.Infrastructure;
using TDP.PushLib.Client.Infrastructure.Configuration;
using TDP.PushLib.Messages;
using TDP.PushLib.Messages.Core;

namespace TDP.PushLib.Client
{
    public class PushClient<T>
    {
        #region "Event definitions"
        public event EventHandler RegisterClientStarted;
        protected virtual void OnRegisterClientStarted(EventArgs e)
        {
            EventHandler handler = RegisterClientStarted;
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

        public delegate void RegisterClientEndedDelegate(object sender, RegisterClientEndedEventArgs<T> e);
        public event RegisterClientEndedDelegate RegisterClientEnded;
        protected virtual void OnRegisterClientEnded(RegisterClientEndedEventArgs<T> e)
        {
            RegisterClientEndedDelegate handler = RegisterClientEnded;
            if (handler != null)
            {
                foreach (RegisterClientEndedDelegate singleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = singleCast.Target as ISynchronizeInvoke;
                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        syncInvoke.Invoke(singleCast, new object[] { this, e });
                    else
                        singleCast(this, e);
                }
            }
        }

        public delegate void PushMessageReceivedDelegate(object sender, PushMessageReceivedEventArgs e);
        public event PushMessageReceivedDelegate PushMessageReceived;
        protected virtual void OnPushMessageReceived(PushMessageReceivedEventArgs e)
        {
            PushMessageReceivedDelegate handler = PushMessageReceived;
            if (handler != null)
            {
                foreach (PushMessageReceivedDelegate singleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = singleCast.Target as ISynchronizeInvoke;
                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        syncInvoke.Invoke(singleCast, new object[] { this, e });
                    else
                        singleCast(this, e);
                }
            }
        }

        public event EventHandler ConnectionClosed;
        protected virtual void OnConnectionClosed(EventArgs e)
        {
            EventHandler handler = ConnectionClosed;
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

        private bool _ClientRunning;
        private Socket _SocketClient;
        private AuthenticationToken<T> _AuthenticationToken;

        public async void ConnectAsync(AuthenticationToken<T> authToken)
        {
            await Task.Run(() =>
            {
                _AuthenticationToken = authToken;
                RegisterClientResponse RegisterClientRsp;

                try
                {
                    EventArgs OnRegisterClientEA = new EventArgs();
                    OnRegisterClientStarted(OnRegisterClientEA);

                    ConfigReader CR = new ConfigReader();
                    _SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress MyIPAddress = IPAddress.Parse(CR.Get(ConfigReaderKeys.KeyConnectIPAddress, Constants.DefaultConnectIPAddress));
                    IPEndPoint MyEndPoint = new IPEndPoint(MyIPAddress, CR.Get(ConfigReaderKeys.KeyConnectPort, Constants.DefaultConnectPort));
                    _SocketClient.Connect(MyEndPoint);

                    RegisterClient<T> RegisterClientMsg = new RegisterClient<T>();
                    RegisterClientMsg.AuthToken = _AuthenticationToken;
                    SocketHelper.SendObject(_SocketClient, RegisterClientMsg);

                    //TODO: Timeout for response?
                    //TODO: Add messages history to response?
                    RegisterClientRsp = (RegisterClientResponse)SocketHelper.ReceiveObject(_SocketClient);

                    if (RegisterClientRsp.Success)
                    {
                        _ClientRunning = true;
                        ManagePushMessagesAsync();
                    }
                }
                catch
                {
                    RegisterClientRsp = new RegisterClientResponse();
                    RegisterClientRsp.ErrorCode = -1;
                    RegisterClientRsp.Success = false;
                    RegisterClientRsp.Message = "Unable to connect to the server.";
                }
                
                RegisterClientEndedEventArgs<T> OnRegisterClientEndedEA = new RegisterClientEndedEventArgs<T>(RegisterClientRsp);
                OnRegisterClientEnded(OnRegisterClientEndedEA);

                return;
            });
        }

        public async void ManagePushMessagesAsync()
        {
            await Task.Run(() =>
            {
                bool ConnectionBroken = false;

                try
                {
                    while (_ClientRunning)
                    {
                        PushMessage pushMessage = SocketHelper.ReceiveObject(_SocketClient) as PushMessage;

                        if (pushMessage == null)
                        {
                            // Connection has been broken
                            _ClientRunning = false;
                            ConnectionBroken = true;
                            break;
                        }

                        PushMessageReceivedEventArgs e = new PushMessageReceivedEventArgs(pushMessage);
                        OnPushMessageReceived(e);
                    }
                }
                catch (ObjectDisposedException)
                {
                    ConnectionBroken = true;
                    System.Diagnostics.Trace.WriteLine("The socket has been closed.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }

                if (ConnectionBroken)
                {
                    EventArgs OnConnectionClosedEA = new EventArgs();
                    OnConnectionClosed(OnConnectionClosedEA);
                }
            });
        }


    }
}

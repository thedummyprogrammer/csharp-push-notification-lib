//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using TDP.PushLib.Messages.Core;

namespace TDP.PushLib.Client.Infrastructure
{
    public class PushMessageReceivedEventArgs : EventArgs
    {
        public PushMessageReceivedEventArgs(PushMessage message)
        {
            PushMessage = message;
        }

        public PushMessage PushMessage { get; set; }
    }
}

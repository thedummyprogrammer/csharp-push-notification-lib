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
    public class RegisterClientEndedEventArgs<T> : EventArgs
    {
        public RegisterClientEndedEventArgs(RegisterClientResponse registerClientResponse)
        {
            RegisterClientResponse = registerClientResponse;
        }

        public RegisterClientResponse RegisterClientResponse { get; set; }
    }
}

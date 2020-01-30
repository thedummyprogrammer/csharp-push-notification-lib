//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using TDP.PushLib.Messages;
using TDP.PushLib.Messages.Core;

namespace TDP.PushLib.Server.Infrastructure
{
    public class InitEndedEventArgs : EventArgs
    {
        public Result Result { get; set; }

        public InitEndedEventArgs(Result result)
        {
            Result = result;
        }
    }
}

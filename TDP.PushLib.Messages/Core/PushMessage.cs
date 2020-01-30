//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using TDP.PushLib.Messages.Abstract;

namespace TDP.PushLib.Messages.Core
{
    [Serializable]
    public class PushMessage : IMessageBase
    {
        public string Message { get; set; }
    }
}

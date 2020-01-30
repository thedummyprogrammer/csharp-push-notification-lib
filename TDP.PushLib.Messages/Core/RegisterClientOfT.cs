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
    public class RegisterClient<T> : IMessageBase
    {
        public AuthenticationToken<T> AuthToken { get; set; }
    }
}

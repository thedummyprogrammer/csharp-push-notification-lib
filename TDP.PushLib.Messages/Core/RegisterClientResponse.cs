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
    public class RegisterClientResponse : IResponseBase
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}

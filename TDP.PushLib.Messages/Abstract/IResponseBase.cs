//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

namespace TDP.PushLib.Messages.Abstract
{ 
    /// <summary>
    /// Base class for message responses
    /// </summary>
    public interface IResponseBase
    {
        bool Success { get; set; }
        int ErrorCode { get; set; }
        string Message { get; set; }
    }
}

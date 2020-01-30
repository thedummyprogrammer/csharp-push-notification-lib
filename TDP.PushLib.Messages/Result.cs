//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

namespace TDP.PushLib.Messages
{
    public class Result
    {
        public Result(int errorCode, bool success, string message)
        {
            ErrorCode = errorCode;
            Success = success;
            Message = message;
        }

        public int ErrorCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

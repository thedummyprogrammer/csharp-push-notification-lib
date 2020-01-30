//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

namespace TDP.BaseServices.Infrastructure
{
    public class MethodResult<T>
    {
        public int ResultCode { get; set; }
        public T ObjectResult { get; set; }

        public MethodResult()
        {

        }

        public MethodResult(int resultCode, T objectResult)
        {
            ResultCode = resultCode;
            ObjectResult = objectResult;
        }
    }
}
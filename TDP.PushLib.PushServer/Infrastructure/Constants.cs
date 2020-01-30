//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

namespace TDP.PushLib.Server.Infrastructure
{
    class Constants
    {
        private Constants()
        {
        }

        public const int DefaultBufferSize = 1024;
        public const string DefaultListenIPAddress = "127.0.0.1";
        public const int DefaultListenPort = 10000;
        public const int DefaultListenBacklog = 5;
        public const bool DefaultUseAuthenticationToken = false;
        public const int AuthenticationTokenDurationMinutes = 1440;
    }
}

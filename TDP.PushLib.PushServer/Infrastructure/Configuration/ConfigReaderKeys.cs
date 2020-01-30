//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

namespace TDP.PushLib.Server.Infrastructure.Configuration
{
    class ConfigReaderKeys
    {
        private ConfigReaderKeys()
        {

        }

        public const string KeyListenIPAddress = "ListenIPAddress";
        public const string KeyListenPort = "ListenPort";
        public const string KeyListenBacklog = "ListenBacklog";
        public const string KeyAuthenticationTokenDurationMinutes = "AuthenticationTokenDurationMinutes";
        public const string KeyValidateAuthenticationToken = "ValidateAuthenticationToken";
    }
}

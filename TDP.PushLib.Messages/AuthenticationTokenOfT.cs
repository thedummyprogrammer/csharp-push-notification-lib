//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using TDP.BaseServices.Infrastructure.Security;

namespace TDP.PushLib.Messages
{
    [Serializable]
    public class AuthenticationToken<T>
    {
        public AuthenticationToken(T clientID, DateTime loginDate, DateTime expireDate, string xmlRSAKey)
        {
            ClientID = clientID;
            LoginDate = loginDate;
            ExpireDate = expireDate;

            if (!string.IsNullOrEmpty(xmlRSAKey))
                SignedData = AsymmetricCryptography.SignData(MakePayLoad(clientID, loginDate, expireDate), xmlRSAKey);
        }

        private string MakePayLoad(T clientID, DateTime loginDate, DateTime expireDate)
        {
            return clientID.ToString() + loginDate.ToString() + expireDate.ToString(); 
        }

        public T ClientID { get; private set; }
        public DateTime LoginDate { get; private set; }
        public DateTime ExpireDate { get; private set; }
        public string SignedData { get; private set; }

        public bool IsValid(string xmlRSAKey, int tokenDuration)
        {
            string TempSignedData = AsymmetricCryptography.SignData(MakePayLoad(ClientID, LoginDate, ExpireDate), xmlRSAKey);

            return (TempSignedData == SignedData) && (LoginDate.AddMinutes(tokenDuration) < ExpireDate);
        }
    }
}

//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using TDP.BaseServices.Infrastructure.Configuration.Abstract;

namespace TDP.BaseServices.Infrastructure.Configuration
{
    public class ConfigReader : IConfigReader
    {
        public string Get(string key, string defaultValue)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public int Get(string key, int defaultValue)
        {
            return int.Parse(System.Configuration.ConfigurationManager.AppSettings[key]);
        }

        public bool Get(string key, bool defaultValue)
        {
            return bool.Parse(System.Configuration.ConfigurationManager.AppSettings[key]);
        }

        public string GetConnectionString(string key)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}
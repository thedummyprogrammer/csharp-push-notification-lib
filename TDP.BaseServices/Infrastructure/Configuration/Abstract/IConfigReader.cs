//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

namespace TDP.BaseServices.Infrastructure.Configuration.Abstract
{
    public interface IConfigReader
    {
        string Get(string key, string defaultValue);

        int Get(string key, int defaultValue);

        string GetConnectionString(string key);
    }
}

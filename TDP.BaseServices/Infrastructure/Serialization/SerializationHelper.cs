//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TDP.BaseServices.Infrastructure.Serialization
{
    public class SerializationHelper
    {
        private SerializationHelper()
        {

        }

        public static byte[] Serialize<T>(T obj)
        {
            BinaryFormatter BF = new BinaryFormatter();

            using (MemoryStream MS = new MemoryStream())
            {
                BF.Serialize(MS, obj);
                return MS.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] objectSerialized)
        {
            BinaryFormatter BF = new BinaryFormatter();

            using (MemoryStream MS = new MemoryStream(objectSerialized))
            {
                return (T)BF.Deserialize(MS);
            }
        }

        public static object Deserialize(byte[] objectSerialized)
        {
            BinaryFormatter BF = new BinaryFormatter();

            using (MemoryStream MS = new MemoryStream(objectSerialized))
            {
                return BF.Deserialize(MS);
            }
        }
    }
}

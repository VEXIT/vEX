/*
 * Author:			Vex Tatarevic
 * Date Created:	2010-08-21
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 *
 */

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
// must add reference to System.ServiceModel.Web and System.Runtime.Serialization 

namespace vEX.Web
{

    /// <summary>
    ///  This class processes JSON. It turns list objects into a JSON string and the other way around
    ///   Ref:
    ///   http://pietschsoft.com/post/2008/02/NET-35-JSON-Serialization-using-the-DataContractJsonSerializer.aspx
    /// </summary>
    public class JSON
    {
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            ms.Dispose();
            return obj;
        }

    }
}

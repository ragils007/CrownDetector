using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Msdfa.Core.Tools
{
	public static class Serializer
	{
		public static void SerializeBinToFile<T>(T objectToSerialize, string fileName)
		{
			Stream stream = File.Open(fileName, FileMode.Create);
			BinaryFormatter bFormatter = new BinaryFormatter();
			bFormatter.Serialize(stream, objectToSerialize);
			stream.Close();
		}

		public static T DeserializeBinFromFile<T>(string fileName)
		{
			Stream stream = File.Open(fileName, FileMode.Open);
			BinaryFormatter bFormatter = new BinaryFormatter();
			T objectToSerialize = (T)bFormatter.Deserialize(stream);
			stream.Close();
			return objectToSerialize;
		}

		public static T DeserializeBinFromByteArray<T>(byte[] byteArray)
		{
			Stream stream = new MemoryStream(byteArray);
			BinaryFormatter bFormatter = new BinaryFormatter();
			T objectToSerialize = (T)bFormatter.Deserialize(stream);
			stream.Close();
			return objectToSerialize;
		}

		public static byte[] SerializeBinToByteArray<T>(T objectToSerialize)
		{
			var stream = new MemoryStream();
			var bFormatter = new BinaryFormatter();
			bFormatter.Serialize(stream, objectToSerialize);
			return stream.ToArray();
		}

	    public static string SerializeToXml<TObjectType>(TObjectType objectToSerialize)
	    {
            var xsSubmit = new XmlSerializer(typeof(TObjectType));
            using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
            using (var writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, objectToSerialize);
                return sww.ToString(); 
            }
        }

	    public static TObjectType DeserializeFromXml<TObjectType>(string xmlData)
	    {
            var byteArray = Encoding.UTF8.GetBytes(xmlData);
	        using (var stream = new MemoryStream(byteArray))
	        {
	            var ser = new XmlSerializer(typeof (TObjectType));
	            return (TObjectType) ser.Deserialize(stream);
	        }
	    }

	    public static void SerializeToXmlFile<TObject>(TObject item, string fullPath, string defaultNamespace)
	    {
	        var xmlSerializer = new XmlSerializer(typeof(TObject), defaultNamespace);

	        fullPath = CheckIfFileExtensionPresent(fullPath, "xml");
	        using (var writer = new StreamWriter(fullPath))
	        {
	            xmlSerializer.Serialize(writer, item);
	        }
	    }

	    public static TObject DeserializeFromXmlFile<TObject>(string fullPath, string defaultNamespace)
	    {
	        var xmlSerializer = new XmlSerializer(typeof(TObject), defaultNamespace);

	        using (var fileStream = new FileStream(fullPath, FileMode.Open))
	        {
	            var item = (TObject) xmlSerializer.Deserialize(fileStream);
	            return item;
	        }
	    }

	    private static string CheckIfFileExtensionPresent(string path, string extension)
	    {
	        var currentExtension = Path.GetExtension(path);
	        if (string.IsNullOrWhiteSpace(currentExtension))
	        {
	            return path + "." + extension;
	        }

	        if (currentExtension != extension)
	        {
	            path = path.Substring(0, path.Length - currentExtension.Length);
	            return path + "." + extension;
	        }

            //prawidłowe rozszerzenie
	        return path;
	    }

	    public static string SerializeToJson<TObject>(TObject obj)
	    {
	        var json = JsonConvert.SerializeObject(obj);
            return json;
	    }

	    public static TObject DeserializeFromJson<TObject>(string json)
	    {
	        if (string.IsNullOrWhiteSpace(json)) return default(TObject);

	        return JsonConvert.DeserializeObject<TObject>(json, new JsonSerializerSettings
	        {
	            Culture = new System.Globalization.CultureInfo("pl-PL")
	        });
	    }

        public static T DeserializeFromJsonToAnonymous<T>(string json, T definition)
        {
            return JsonConvert.DeserializeAnonymousType<T>(json, definition);
        }

        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }
    }
}

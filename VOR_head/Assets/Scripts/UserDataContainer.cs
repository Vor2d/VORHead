using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("UserDataCollection")]
public class UserDataContainer  {
	[XmlArray("UsersData")]
	[XmlArrayItem("UserData")]
	public UserData[] UsersData;

	public static UserDataContainer Load(string path){
		var serializer = new XmlSerializer(typeof(UserDataContainer));
		using(var stream = new FileStream(path, FileMode.Open))
 		{
 			return serializer.Deserialize(stream) as UserDataContainer;
 		}
	}

	public static UserDataContainer LoadFromText(string text){
		var serializer = new XmlSerializer(typeof(UserDataContainer));
		return serializer.Deserialize(new StringReader(text)) as UserDataContainer;
	}



	public void Save(string path){
		var serializer = new XmlSerializer(typeof(UserDataContainer));
		using(var stream = new FileStream(path, FileMode.Append))
 		{
 			serializer.Serialize(stream, this);
 		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class UserData {
	[XmlAttribute("name")]
	public string Name;

	public string Date;
	public double StaticAcurity;
	public double DynamicAcurity;
	public double HeadSpeed;


}

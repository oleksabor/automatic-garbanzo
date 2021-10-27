using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FluentFtpMT
{
	/// <summary>
	/// contains web application url, paths to get and remove files, auth data
	/// </summary>
	public class WebSettings
	{
		public string Url { get; set; }
		public int Port { get; set; }
		public string PathGet { get; set; }
		public string PathDel { get; set; }
		[XmlAttribute("formats")]
		public string Formats { get; set; }

		[XmlAttribute("user")]
		public string User { get; set; }
		[XmlAttribute("password")]
		public string Password { get; set; }
		public bool DetailedLog { get; set; }

		[XmlAttribute("tlsVersion")]
		public string TlsProtocolVersion { get; set; }
		[XmlAttribute("ignoreCertificateError")]
		public bool IgnoreCertificateError { get; set; }

		[XmlAttribute("fastPooling")]
		public bool FastPooling { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentFtpMT
{
	//
	// Summary:
	//     helper class to store file information
	public class Image
	{
		//
		// Summary:
		//     Initializes a new instance of the DirectSync.Interfaces.Image class.
		public Image()
		{ }
		//
		// Summary:
		//     Initializes a new instance of the DirectSync.Interfaces.Image class.
		//
		// Parameters:
		//   name:
		//     The name.
		//
		//   value:
		//     The value.
		public Image(string name, byte[] value)
		{
			Name = name;
			Value = value;
		}

		//
		// Summary:
		//     Gets or sets the name.
		//
		// Value:
		//     The file name from file storage.
		public string Name { get; set; }
		//
		// Summary:
		//     Gets or sets the value.
		//
		// Value:
		//     The image value decrypted.
		public byte[] Value { get; set; }
	}
}

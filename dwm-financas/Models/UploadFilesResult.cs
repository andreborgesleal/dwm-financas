using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUploadMVC4.Models
{
	public class UploadFilesResult
	{
		public string Name { get; set; }
		public int Length { get; set; }
		public string Type { get; set; }
	}
}

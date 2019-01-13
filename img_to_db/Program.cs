using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace img_to_db
{
	class Program
	{
		public static String[] types = new string[] { "png","bmp","jpg","jpeg","tif","gif","tiff" };
		public static List<string> files = new List<string>();

		[STAThread]
		static void Main(string[] args)
		{
			if(args.Length == 0) { Console.WriteLine("No folders.\r\nSet folder using -dir"); }
			else if(args[0] == "-dir")
			{
				foreach (string fil in Directory.GetFiles(args[1]))
				{
					string ex = fil.Substring(fil.LastIndexOf(".") + 1);
					if (types.Contains(ex.ToLower()))
					{
						files.Add(fil);
					}
				}
			}

			if(files.Count > 0) { generateSQLiteInsert();  }
		}

		public static void generateSQLiteInsert()
		{
			string template = "\r\n('{NAME}', {SIZE}, '{FOR}', '{IMAGE}'), ";
			StringBuilder sb = new StringBuilder();
			sb.Append("INSERT INTO images (name, size, for, image) VALUES ");

			foreach (string s in files)
			{
				string name = "", size = "", _for = "", image = "";

				name	= s.Substring(s.LastIndexOf("\\") + 1, s.LastIndexOf(".") - s.LastIndexOf("\\") - 1);
				size	= s.Substring(s.LastIndexOf("_") + 1, s.LastIndexOf(".") - s.LastIndexOf("_") - 1);
				_for	= s.Substring(s.LastIndexOf("\\") + 1);
				_for	= _for.Substring(0, _for.IndexOf("_"));
				image	= getImageCode(s);

				sb.Append(template
					.Replace("{NAME}",	name)
					.Replace("{SIZE}",	size)
					.Replace("{FOR}",	_for)
					.Replace("{IMAGE}",	image));
			}

			string output = sb.ToString();
			output = output.Trim(new char[] { ' ', ',' });
			output += ";";

			Console.WriteLine("Save File? (Y/N)");
			string yn = "";
			while (true)
			{
				yn = Console.ReadLine().Trim();
				if (new string[] { "y","n" }.Contains(yn.ToLower())) { break; }
			}
			if(yn.ToLower() == "y")
			{
				File.Create("output.txt").Close();
				File.WriteAllText("output.txt", output);
				System.Diagnostics.Process.Start("output.txt");
			}

			Console.WriteLine("Copy to clipboard? (Y/N)");
			yn = "";
			while (true)
			{
				yn = Console.ReadLine().Trim();
				if (new string[] { "y", "n" }.Contains(yn.ToLower())) { break; }
			}

			if(yn.ToLower() == "y") { Clipboard.SetText(output); }
		}

		public static string getImageCode(string path)
		{
			string img = "";

			Bitmap bmp = new Bitmap(path);
			ImageConverter ic = new ImageConverter();
			byte[] bytes = ((byte[])ic.ConvertTo(bmp, typeof(byte[])));
			img = String.Join("|", bytes);
			
			return img;
		}

	}
}

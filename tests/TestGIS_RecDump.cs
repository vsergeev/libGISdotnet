/*
 * TestGIS_RecDump prints all of the records stored in an Atmel Generic, Intel
 * HEX, or Motorola S-Record formatted file.
 *
 * Authors: Jerry G. Scherer <scherej1@hotmail.com>,
 *          Vanya A. Sergeev <vsergeev@gmail.com>
 */

using System;
using System.Text;
using System.IO;
using LibGIS.Net;

namespace TestLibGISdotNet
{
	class Program
	{
		static void Main(string[] args)
		{
			AtmelGeneric generic;
			IntelHex ihex;
			SRecord srec;
			StreamReader sr = null;

			if (args.Length < 2)
			{
				Console.WriteLine("Usage: TestGIS_RecDump.exe <file format> <file>");
				Console.WriteLine("This program will print the records saved in a generic, Intel HEX, or Motorola\nS-Record formatted file.\n");
				Console.WriteLine("  <file format> can be generic, ihex, or srecord.");
				Console.WriteLine("  <file> is the path to the formatted object file.");
				Environment.Exit(-1);
			}

			try
			{
				sr = new StreamReader(args[1]);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error opening file: " + e.Message);
				Environment.Exit(-1);
			}

			if (string.Compare(args[0], "generic") == 0)
			{
				generic = new AtmelGeneric();
				AtmelGenericStructure gen_s;
				while (true)
				{
					gen_s = generic.Read(sr);
					if (gen_s != null)
						Console.WriteLine(generic.Print(true));
					else
						break;
				}

			}
			else if (string.Compare(args[0], "ihex") == 0)
			{
				ihex = new IntelHex();
				IntelHexStructure ihex_s;
				while (true)
				{
					ihex_s = ihex.Read(sr);
					if (ihex_s != null)
						Console.WriteLine(ihex.Print(true));
					else
						break;
				}
			}
			else if (string.Compare(args[0], "srecord") == 0)
			{
				srec = new SRecord();
				SRecordStructure srec_s;
				while (true)
				{
					srec_s = srec.Read(sr);
					if (srec_s != null)
						Console.WriteLine(srec.Print(true));
					else
						break;
				}
			}
			else
			{
				Console.WriteLine("Unknown file format specified!");
				sr.Close();
				Environment.Exit(-1);
			}

			sr.Close();
		}
	}
}


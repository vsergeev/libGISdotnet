/*
 * TestGIS_Write writes some test Atmel Generic, Intel HEX, and Motorola
 * S-Record records to files and reads them back to the standard output,
 * using LibGIS.Net.
 *
 * Authors: Jerry G. Scherer <scherej1@hotmail.com>,
 *	    Vanya A. Sergeev <vsergeev@gmail.com>
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
			byte[] data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
			UInt16[] data16 = new UInt16[] { 0x0101, 0x0202, 0x0303, 0x0404 };

			TestAtmelGeneric("test.gen", data16);
			Console.WriteLine();

			TestIntelHex("test.ihx", data);
			Console.WriteLine();

			TestSRecord("TestSRecord", "test.scd", data);
			Console.WriteLine();
		}

		private static void TestAtmelGeneric(string fileName, UInt16[] data16)
		{
			UInt16 address = 0;
			AtmelGeneric grec = new AtmelGeneric();
			StreamWriter sw = new StreamWriter(fileName);

			// Create the first record with data
			AtmelGenericStructure grecs = grec.NewRecord(address++, data16[0]);
			grec.Write(sw);

			// Create another record with the next data point
			grec.NewRecord(address++, data16[1]);
			grec.Write(sw);

			// Create another record with the next data point
			grec.NewRecord(address++, data16[2]);
			grec.Write(sw);

			// Create the last data record with the last data point
			grec.NewRecord(address++, data16[3]);
			grec.Write(sw);
			sw.Close();

			Console.WriteLine("Wrote Atmel Generic formatted file: " + fileName);
			Console.WriteLine("Reading back Atmel Generic file:");

			// Open up the new file and attempt to read the records and print to the console
			StreamReader sr = new StreamReader(fileName);
			grecs = grec.Read(sr);
			Console.WriteLine((grecs != null) ? grec.Print() : "Could not read record!");
			grecs = grec.Read(sr);
			Console.WriteLine((grecs != null) ? grec.Print() : "Could not read record!");
			grecs = grec.Read(sr);
			Console.WriteLine((grecs != null) ? grec.Print() : "Could not read record!");
			grecs = grec.Read(sr);
			Console.WriteLine((grecs != null) ? grec.Print() : "Could not read record!");
			sr.Close();
		}

		private static void TestIntelHex(string fileName, byte[] data)
		{
			IntelHex irec = new IntelHex();
			StreamWriter sw = new StreamWriter(fileName);

			// Create a new type-0 record with data
			IntelHexStructure irecs = irec.NewRecord(0, 0, data, data.Length);
			irec.Write(sw);

			// Create another type-0 record with new data
			irec.NewRecord(0, 8, data, data.Length);
			irec.Write(sw);

			// Create another type-0 record with new data
			irec.NewRecord(0, 16, data, data.Length);
			irec.Write(sw);

			// Create an end of record type-1 record
			irec.NewRecord(1, 0, null, 0);
			irec.Write(sw);
			sw.Close();

			Console.WriteLine("Wrote Intel HEX formatted file: " + fileName);
			Console.WriteLine("Reading back Intel HEX file:");

			// Open up the new file and attempt to read the records and print to the console
			StreamReader sr = new StreamReader(fileName);
			irecs = irec.Read(sr);
			Console.WriteLine((irecs != null) ? irec.Print() : "Could not read record!");
			irecs = irec.Read(sr);
			Console.WriteLine((irecs != null) ? irec.Print() : "Could not read record!");
			irecs = irec.Read(sr);
			Console.WriteLine((irecs != null) ? irec.Print() : "Could not read record!");
			irecs = irec.Read(sr);
			Console.WriteLine((irecs != null) ? irec.Print() : "Could not read record!");
			sr.Close();
		}

		private static void TestSRecord(String title, String fileName, byte[] data)
		{
			SRecord srec = new SRecord();
			StreamWriter sw = new StreamWriter(fileName);

			// Create a new S0 record with the title
			SRecordStructure srecs = srec.NewRecord(0, 0, Encoding.ASCII.GetBytes(title), title.Length);
			srec.Write(sw);

			// Create a S1 data record
			srec.NewRecord(1, 0, data, data.Length);
			srec.Write(sw);

			// Create a S5 transmission record
			srec.NewRecord(5, 1, null, 0);
			srec.Write(sw);

			// Create a S9 program start record
			srec.NewRecord(9, 0, null, 0);
			srec.Write(sw);
			sw.Close();

			Console.WriteLine("Wrote Motorola S-Record formatted file: " + fileName);
			Console.WriteLine("Reading back Motorola S-Record file:");

			// Open up the new file and attempt to read the records and print to the console
			StreamReader sr = new StreamReader(fileName);
			srecs = srec.Read(sr);
			Console.WriteLine((srecs != null)? srec.Print() : "Could not read record!");
			srecs = srec.Read(sr);
			Console.WriteLine((srecs != null) ? srec.Print() : "Could not read record!");
			srecs = srec.Read(sr);
			Console.WriteLine((srecs != null) ? srec.Print() : "Could not read record!");
			srecs = srec.Read(sr);
			Console.WriteLine((srecs != null) ? srec.Print() : "Could not read record!");
			sr.Close();
		}

	}
}

/*
 *  AtmelGeneric.cs
 *  Utility class to create, read, write, and print Atmel Generic binary records.
 *
 *  Original Written by Vanya A. Sergeev <vsergeev@gmail.com>
 *  Version 1.0.5 - February 2011
 *
 *  Modified by Jerry G. Scherer <scherej1@hotmail.com> for C#
 *  Version 1.0.0 - May 2012
 */
using System;
using System.Text;
using System.IO;

namespace LibGIS.Net
{
	/// <summary>
	/// AtmelGenericStructure provides the internal data structure which will be used by the AtmelGeneric class.
	/// This class is used for internal processing and is declared public to allow the application that instantiates
	/// the AtmelGeneric class access to the internal storage.
	/// </summary>
	public class AtmelGenericStructure
	{
		public UInt32 address; 	//< The 24-bit address field of the record.
		public UInt16 data; 	//< The 16-bit data field of the record.
	}

	/// <summary>
	/// AtmelGeneric is the base class to work with Atmel Generic records.
	/// This class will contain all necessary functions to process data using the Atmel Generic record standard.
	/// </summary>
	public class AtmelGeneric
	{
		// 16 should be plenty of space to read in an Atmel Generic record (11 bytes in reality)
		const int ATMEL_GENERIC_RECORD_BUFF_SIZE = 16;
		// Offsets and lengths of various fields in an Atmel Generic record
		const int ATMEL_GENERIC_ADDRESS_LEN = 6;
		const int ATMEL_GENERIC_DATA_LEN = 4;
		// The separator character, a colon, that separates the data and address fields in the Atmel Generic records
		const int ATMEL_GENERIC_SEPARATOR_OFFSET = 6;
		const char ATMEL_GENERIC_SEPARATOR = ':';

		const int ATMEL_GENERIC_OK = 0; 				        //< Error code for success or no error.
		const int ATMEL_GENERIC_ERROR_FILE = -1; 			    //< Error code for error while reading from or writing to a file. You may check errno for the exact error if this error code is encountered.
		const int ATMEL_GENERIC_ERROR_EOF = -2; 			    //< Error code for encountering end-of-file when reading from a file.
		const int ATMEL_GENERIC_ERROR_INVALID_RECORD = -3; 	    //< Error code for error if an invalid record was read.
		const int ATMEL_GENERIC_ERROR_INVALID_ARGUMENTS = -4; 	//< Error code for error from invalid arguments passed to function.
		const int ATMEL_GENERIC_ERROR_NEWLINE = -5; 		    //< Error code for encountering a newline with no record when reading from a file.
		const int ATMEL_GENERIC_ERROR_INVALID_STRUCTURE = -6;   //< Error code for not building a structure prior to calling the function.

		AtmelGenericStructure grec = new AtmelGenericStructure();   // internal structure that holds the record information.
		int status = ATMEL_GENERIC_ERROR_INVALID_ARGUMENTS;         // internal variable that saves the status of the last function call.

		// Accessor variable to return status of last function call.
		public int Status
		{
			get { return status; }
		}


		/// <summary>
		/// Initializes a new AtmelGenericRecord structure that the parameter genericRecord points to with the passed
		/// 24-bit integer address, and 16-bit data word.
		/// </summary>
		/// <param name="address">The 24-bit address of the record.</param>
		/// <param name="data">The 16-bit data word</param>
		/// <returns>AtmelGenericStructure instance or null, if null then query Status class variable for the error.</returns>
		public AtmelGenericStructure NewRecord(UInt32 address, UInt16 data)
		{
			// Assert genericRecord structure
			if (grec == null)
			{
				status = ATMEL_GENERIC_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			grec.address = address;
			grec.data = data;

			status = ATMEL_GENERIC_OK;
			return grec;
		}

		/// <summary>
		/// Utility function to read an Atmel Generic record from a file
		/// </summary>
		/// <param name="inStream">An instance of the StreamReader class to allow reading the file data.</param>
		/// <returns>AtmelGenericStructure instance or null, if null then query Status class variable for the error.</returns>
		public AtmelGenericStructure Read(StreamReader inStream)
		{
			String recordBuff;

			// Check our record pointer and file pointer
			if (grec == null || inStream == null)
			{
				status = ATMEL_GENERIC_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			try
			{
				// Read Line will return a line from the file.
				recordBuff = inStream.ReadLine();
			}
			catch (Exception)
			{
				status = ATMEL_GENERIC_ERROR_FILE;
				return null;
			}

			// Check if we hit a newline
			if (recordBuff == null || recordBuff.Length == 0)
			{
				status = ATMEL_GENERIC_ERROR_NEWLINE;
				return null;
			}

			// Size check that the record has the address, data, and start code
			if (recordBuff.Length < ATMEL_GENERIC_ADDRESS_LEN + ATMEL_GENERIC_DATA_LEN + 1)
			{
				status = ATMEL_GENERIC_ERROR_INVALID_RECORD;
				return null;
			}

			// Check for the record "start code" (the colon that separates the address and data
			if (recordBuff[ATMEL_GENERIC_SEPARATOR_OFFSET] != ATMEL_GENERIC_SEPARATOR)
			{
				status = ATMEL_GENERIC_ERROR_INVALID_RECORD;
				return null;
			}

			// Replace the colon "start code" with a 0 so we can convert the ASCII hex encoded
			// address up to this point
			grec.address = Convert.ToUInt32(recordBuff.Substring(0, ATMEL_GENERIC_SEPARATOR_OFFSET), 16);

			// Convert the rest of the data past the colon, this string has been null terminated at
			// the end already
			 grec.data = Convert.ToUInt16(recordBuff.Substring(ATMEL_GENERIC_SEPARATOR_OFFSET+1), 16);

			status = ATMEL_GENERIC_OK;
			return grec;
		}

		/// <summary>
		/// Utility function to write an Atmel Generic record to a file
		/// </summary>
		/// <param name="outStream">An instance of the StreamWriter class to allow writing the file data.</param>
		/// <returns>AtmelGenericStructure instance or null, if null then query Status class variable for the error.</returns>
		public AtmelGenericStructure Write(StreamWriter outStream)
		{
			// Check our record pointer and file pointer
			if (grec == null || outStream == null)
			{
				status = ATMEL_GENERIC_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			try
			{
				outStream.WriteLine(String.Format("{0:X6}{1}{2:X4}",  grec.address, ATMEL_GENERIC_SEPARATOR, grec.data));
			}
			catch(Exception)
			{
				status = ATMEL_GENERIC_ERROR_FILE;
				return null;
			}

			status = ATMEL_GENERIC_OK;
			return grec;
		}

		/// <summary>
		/// Utility function to write an Atmel Generic record to a file
		/// </summary>
		/// <param name="verbose">A boolean set to false by default, if set to true will provide extended information.</param>
		/// <returns>String which provides the output of the function, this does not write directly to the console.</returns>
		public String Print(bool verbose = false)
		{
			String returnString;

			if (grec == null)
			{
				status = ATMEL_GENERIC_ERROR_INVALID_STRUCTURE;
				return null;
			}

			if (verbose)
			{
				returnString = String.Format("Atmel Generic Address: \t0x{0:X6}\n", grec.address);
				returnString += String.Format("Atmel Generic Data: \t0x{0:X4}\n", grec.data);
			}
			else
			{
				returnString = String.Format("{0:X6}{1}{2:X4}", grec.address, ATMEL_GENERIC_SEPARATOR, grec.data);
			}

			status = ATMEL_GENERIC_OK;
			return (returnString);
		}

	}
}

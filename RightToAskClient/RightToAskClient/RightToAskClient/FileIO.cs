using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using RightToAskClient.Models;

namespace RightToAskClient
{
	public static class FileIO
	{
		public static Result<T> ReadDataFromStoredJson<T>(string filename, JsonSerializerOptions jsonSerializerOptions)
		{
			T deserializedData;

			try
			{
				var streamResult = TryToGetFileStream(filename);
				if (!String.IsNullOrEmpty(streamResult.Err))
				{
					return new Result<T>() { Err = streamResult.Err };
				}
				
				using (var sr = new StreamReader(streamResult.Ok))
				{
					string dataString = sr.ReadToEnd();
					deserializedData = (T)JsonSerializer.Deserialize<T>(dataString, jsonSerializerOptions);
				}
			}
			catch (IOException e)
			{
				Console.WriteLine("File could not be read: " + filename);
				Console.WriteLine(e.Message);
				return new Result<T>() { Err = e.Message };
			}
			catch (JsonException e)
			{
				Console.WriteLine("JSON could not be deserialised: " + filename);
				Console.WriteLine(e.Message);
				return new Result<T>() { Err = e.Message };
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + filename);
				Console.WriteLine(e.Message);
				return new Result<T>() { Err = e.Message };
			}

			if (deserializedData is null)
			{
				string error = "Error: Could not deserialize" + filename;
				Console.WriteLine(error);
				return new Result<T>() { Err = error };
			}

			return new Result<T>() { Ok = deserializedData };
		}

		
		public static Result<string> ReadFirstLineOfFileAsString(string filename)
		{
			try
			{
				/*
				var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadingContext)).Assembly;
				Stream? stream = assembly.GetManifestResourceStream("RightToAskClient.Resources." + filename);
				if (stream is null)
				{
					Console.WriteLine("Could not find file: " + filename);
					return new Result<string>() { Err = "Could not find file: " + filename};
				}
				*/
				var streamResult = TryToGetFileStream(filename);
				if (!String.IsNullOrEmpty(streamResult.Err))
				{
					return new Result<string>() { Err = streamResult.Err };
				}
				
				using (var sr = new StreamReader(streamResult.Ok))
				{
					string data = sr.ReadLine() ?? string.Empty;
					if (!String.IsNullOrEmpty(data))
					{
						return new Result<string>() { Ok = data };
					}
				}

			}
			catch (IOException e)
			{
				Console.WriteLine("File could not be read: " + filename);
				Console.WriteLine(e.Message);
			}

			return new Result<string>() { Err = "Error reading file: " + filename };
		}

		public static void ReadDataFromCSV<T>(string filename, List<T> MPCollection, Func<string, T> parseLine)
		{
			try
			{
				var streamResult = TryToGetFileStream(filename);
				if (!String.IsNullOrEmpty(streamResult.Err))
				{
					return;
				}
				
				using (var sr = new StreamReader(streamResult.Ok))
				{
					// Read the first line, which just has headings we can ignore.
					sr.ReadLine();
					string? line;
					while ((line = sr.ReadLine()) != null)
					{
						var MPToAdd = parseLine(line);
						if (MPToAdd != null)
						{
							MPCollection.Add(MPToAdd);
						}
					}
				}
			}
			catch (IOException e)
			{
				Console.WriteLine("MP file could not be read: " + filename);
				Console.WriteLine(e.Message);
			}

		}
		private static Result<Stream> TryToGetFileStream(string filename)
		{
				var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadingContext)).Assembly;
				Stream? stream = assembly.GetManifestResourceStream("RightToAskClient.Resources." + filename);
				if (stream is null)
				{
					Console.WriteLine("Could not find file: " + filename);
					return new Result<Stream>() { Err = "Could not find file: " + filename};
				}

				return new Result<Stream> { Ok = stream };
		}
	}
}


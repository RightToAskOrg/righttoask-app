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
		public static Result<T> readDataFromStoredJSON<T>(string filename, JsonSerializerOptions jsonSerializerOptions)
		{
			T deserializedData;

			try
			{
				var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadingContext)).Assembly;
				Stream stream = assembly.GetManifestResourceStream("RightToAskClient.Resources." + filename);
				using (var sr = new StreamReader(stream))
				{
					string? dataString = sr.ReadToEnd();
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
				string data;
				var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadingContext)).Assembly;
				Stream stream = assembly.GetManifestResourceStream("RightToAskClient.Resources." + filename);
				using (var sr = new StreamReader(stream))
				{
					data = sr.ReadLine() ?? string.Empty;
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

		public static void readDataFromCSV<T>(string filename, List<T> MPCollection, Func<string, T> parseLine)
		{
			string line;
			try

			{
				T MPToAdd;
				var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadingContext)).Assembly;
				Stream stream = assembly.GetManifestResourceStream("RightToAskClient.Resources." + filename);
				using (var sr = new StreamReader(stream))
				{
					// Read the first line, which just has headings we can ignore.
					sr.ReadLine();
					while ((line = sr.ReadLine()) != null)
					{
						MPToAdd = parseLine(line);
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
	}
}


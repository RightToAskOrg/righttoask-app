using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using RightToAskClient.Models;

namespace RightToAskClient.Helpers
{
	public static class FileIO
	{
		public static JOSResult<T> ReadDataFromStoredJson<T>(string filename,
			JsonSerializerOptions jsonSerializerOptions)
		{
			T deserializedData;

			try
			{
				var streamResult = TryToGetFileStream(filename);
				if (streamResult.Success)
				{
					using var sr = new StreamReader(streamResult.Data);
					var dataString = sr.ReadToEnd();
					deserializedData = (T)JsonSerializer.Deserialize<T>(dataString, jsonSerializerOptions);
					if (deserializedData is { })
					{
						return new SuccessResult<T>(deserializedData);
					}
				}
				else if (streamResult is ErrorResult<Stream> error)
				{
					return new ErrorResult<T>(error.Message);
				}

				// At the moment, this never happens because the two cases above are exhaustive.
				var newError = "Error: Could not deserialize" + filename;
				Debug.WriteLine(newError);
				return new ErrorResult<T>(newError);
			}
			catch (IOException e)
			{
				Debug.WriteLine("File could not be read: " + filename);
				Debug.WriteLine(e.Message);
				return new ErrorResult<T>(e.Message);
			}
			catch (JsonException e)
			{
				Debug.WriteLine("JSON could not be deserialised: " + filename);
				Debug.WriteLine(e.Message);
				return new ErrorResult<T>(e.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Error: " + filename);
				Debug.WriteLine(e.Message);
				return new ErrorResult<T>(e.Message);
			}
		}

		
		public static JOSResult<string> ReadFirstLineOfFileAsString(string filename)
		{
			try
			{
				var streamResult = TryToGetFileStream(filename);
				if (streamResult.Success)
				{
					using var sr = new StreamReader(streamResult.Data);
					var data = sr.ReadLine(); 
					if (data != null)
					{
						return new SuccessResult<string>(data);
					}
				} else if (streamResult is ErrorResult<Stream> result)
				{
					return new ErrorResult<string>(result.Message);
				}
			}
			catch (IOException e)
			{
				Debug.WriteLine("Error reading file: " + filename);
				Debug.WriteLine(e.Message);
			}

			return new ErrorResult<string>("Error reading file: " + filename);
		}

		public static void ReadDataFromCsv<T>(string filename, List<T> MPCollection, Func<string, T> parseLine)
		{
			try
			{
				var streamResult = TryToGetFileStream(filename);
				if (streamResult.Failure)
				{
					return;
				}

				using var sr = new StreamReader(streamResult.Data);
				// Read the first line, which just has headings we can ignore.
				sr.ReadLine();
				while (sr.ReadLine() is { } line)
				{
					var MPToAdd = parseLine(line);
					if (MPToAdd != null)
					{
						MPCollection.Add(MPToAdd);
					}
				}
			}
			catch (IOException e)
			{
				Debug.WriteLine("MP file could not be read: " + filename);
				Debug.WriteLine(e.Message);
			}

		}
		private static JOSResult<Stream> TryToGetFileStream(string filename)
		{
				var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ReadingContext)).Assembly;
				var stream = assembly.GetManifestResourceStream("RightToAskClient.Resources." + filename);
				if (stream is null)
				{
					Debug.WriteLine("Could not find file: " + filename);
					return new ErrorResult<Stream>("Could not find file: " + filename);
					
				}

				return new SuccessResult<Stream>(stream);
		}
	}
}


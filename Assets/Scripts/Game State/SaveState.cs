using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;


public class SaveState {
	public static BinaryFormatter binaryFormatter = new BinaryFormatter();
	public static BinaryFormatter bf = new BinaryFormatter();
	public static string user = System.Environment.GetEnvironmentVariable("UserName");
	//string root = Path.GetPathRoot(Environment.SystemDirectory);
	//Hardcoded root since Environment.SystemDirectory does not seem to be defined on unity
	public static string savePath = @"C:\Users\" + user + @"\Documents\XCOMUFO\";
	
	public static String serializeStr(object serializableObject) {
		MemoryStream memoryStream = new MemoryStream();
		bf.Serialize(memoryStream, serializableObject);
		return System.Convert.ToBase64String(memoryStream.ToArray());
	}
    
	public static object deserializeStr(string byteArray) {
		MemoryStream memoryStream = new MemoryStream(System.Convert.FromBase64String(byteArray));
		return bf.Deserialize(memoryStream);
	}
	
	public static byte[] SerializeToArray(gameManager request) {
		byte[] result;
		BinaryFormatter serializer = new BinaryFormatter();
		using (MemoryStream memStream = new MemoryStream()) {
			serializer.Serialize(memStream, request);
			result = memStream.GetBuffer();
		}
		return result;
	}
	
	public static void save(string saveFile) {
		if(!Directory.Exists(savePath)) {
			Directory.CreateDirectory(savePath);
		}
		SavedGameState sav = new SavedGameState();
		FileStream fs = new FileStream(savePath+saveFile, FileMode.Create);
		
		try {
			binaryFormatter.Serialize(fs, sav);  
		}  
		catch (SerializationException ex) {  
			throw new ApplicationException("The object graph could not be serialized", ex);  
		}
		finally {  
			fs.Close();  
		}
	}
	
	public static SavedGameState load(string loadFile) {
		FileStream fs = new FileStream(savePath+loadFile, FileMode.Open);
		SavedGameState result;
		try{
			result = (SavedGameState)binaryFormatter.Deserialize(fs);
		}
		catch (SerializationException ex) {
			throw new ApplicationException("The object graph could not be deserialized", ex);  
		}
		finally {
			fs.Close();
		}
		return result;
	}
}
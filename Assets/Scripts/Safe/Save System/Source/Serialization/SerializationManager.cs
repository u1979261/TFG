using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;


public class SerializationManager : MonoBehaviour
{
    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();


        // CHECK IF THE SAVE PATH EXISTS IF IT DOESNT CREATE IT
        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.dataPath + "/saves");
        }


        // THE PATH & NAME OF THE ACTUAL SAVE FILE
        string path = Application.dataPath + "/saves/" + saveName + ".save";

        FileStream file = File.Create(path);


        formatter.Serialize(file, saveData);

        file.Close();


        return true;

    }

    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.Log("SAVE SYSTEM : Save file does not exist.");
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            Debug.Log("SAVE SYSTEM : Save file Loaded Successfully");
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("SAVE SYSTEM : Failed to load save located at {0}", path);
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector3_Surrogate vector3_Surrogate = new Vector3_Surrogate();
        Quaternion_Surrogate quaternion_Surrogate = new Quaternion_Surrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3_Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternion_Surrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}

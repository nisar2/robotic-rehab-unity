using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class TrajectorySaver
{
    public TrajectoryData LoadTrajectory(string filename)
    {
        string jsonString = LoadJsonFromFile(filename);

        // Optionally, deserialize the JSON string back to an object
        TrajectoryData deserializedTrajData = JsonConvert.DeserializeObject<TrajectoryData>(jsonString);

        return deserializedTrajData;
    }

    public void SaveTrajectory(TrajectoryData trajData, string filename)
    {
        // Serialize the object to a JSON string
        string jsonString = JsonConvert.SerializeObject(trajData, Formatting.Indented);

        // Output the JSON string to the console
        Debug.Log(jsonString);

        SaveJsonToFile(jsonString, filename);
    }
    
    void SaveJsonToFile(string jsonString, string filename)
    {
        // Set the file path to save the JSON data
        string filePath = Path.Combine(Path.Combine(Application.dataPath, "trajectories"), $"{filename}.json");
        // Write the JSON string to a file
        File.WriteAllText(filePath, jsonString);
        Debug.Log($"JSON data saved to {filePath}");
    }

    string LoadJsonFromFile(string filename)
    {
        // Set the file path to save the JSON data
        string filePath = Path.Combine(Path.Combine(Application.dataPath, "trajectories"), $"{filename}.json");
        // Read the JSON string from the file
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            Debug.Log($"JSON data loaded from {filePath}");
            return jsonString;
        }
        else
        {
            Debug.LogError("File not found");
            return null;
        }
    }
}

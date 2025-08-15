using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputObjectiveMetricsPage : MonoBehaviour
{
    public UnityEvent onStartTherapy = new UnityEvent();
    [SerializeField] private UnityEvent<string> OnError = new UnityEvent<string>();

    [SerializeField] private ObjectiveMetricInputField[] objectiveMetricInputFields;
    [SerializeField] private TextMeshProUGUI instructionText;

    //private ParticipantIdList participantList;
    private ObjectiveMetrics newObjectiveMetrics;

    private ObjectiveMetrics lastObjectiveMetrics;

    private void OnEnable()
    {
        Debug.Log("HERE");
        objectiveMetricInputFields = GetComponentsInChildren<ObjectiveMetricInputField>();
        DirectoryInfo directoryInfo = new DirectoryInfo(DataManager.Instance.GetSite().DataPath + "/" + DataManager.Instance.GetSessionParticipant().Data + "/obj_metrics");
        FileInfo[] files = directoryInfo.GetFiles();

        if (files.Length == 0)
        {
            foreach (ObjectiveMetricInputField objectiveMetricInputField in objectiveMetricInputFields)
            {
                objectiveMetricInputField.ManuallyUpdate(null);
            }
            newObjectiveMetrics = new ObjectiveMetrics();
            return;
        }

        var sortedFiles = files.OrderBy(file => file.CreationTime);

        FileInfo mostRecentFile = sortedFiles.Last();

        // participant list json file exists
        string json = File.ReadAllText(mostRecentFile.FullName);
        ObjectiveMetrics objMetrics = JsonConvert.DeserializeObject<ObjectiveMetrics>(json);
        lastObjectiveMetrics = objMetrics;
        newObjectiveMetrics = new ObjectiveMetrics();

        foreach (ObjectiveMetricInputField objectiveMetricInputField in objectiveMetricInputFields)
        {
            string key = objectiveMetricInputField.GetKey();
            if (objMetrics.ObjectiveMetricCollection.ContainsKey(key))
            {
                float value = objMetrics.ObjectiveMetricCollection[key];
                objectiveMetricInputField.ManuallyUpdate(value);
                UpdateObjectiveMetricCollection(key, value);
            }
        }
    }

    public bool CheckIfLastObjMetricsSameAsNew()
    {
        if (lastObjectiveMetrics == null) return false;
        foreach (KeyValuePair<string, float> entry in newObjectiveMetrics.ObjectiveMetricCollection)
        {
            if (!lastObjectiveMetrics.ObjectiveMetricCollection.ContainsKey(entry.Key))
            {
                return false;
            }
            if (newObjectiveMetrics.ObjectiveMetricCollection[entry.Key] != lastObjectiveMetrics.ObjectiveMetricCollection[entry.Key])
            {
                return false;
            }
        }
        foreach (KeyValuePair<string, float> entry in lastObjectiveMetrics.ObjectiveMetricCollection)
        {
            if (!newObjectiveMetrics.ObjectiveMetricCollection.ContainsKey((string)entry.Key))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearAll()
    {
        Debug.Log("HERE");
        foreach (ObjectiveMetricInputField objectiveMetricInputField in objectiveMetricInputFields)
        {
            objectiveMetricInputField.ManuallyUpdate(null);
        }
        newObjectiveMetrics.ObjectiveMetricCollection.Clear();
        
    }

    public void UpdateObjectiveMetricCollection(string key, float data)
    {
        Debug.Log(data);
        if (newObjectiveMetrics.ObjectiveMetricCollection.ContainsKey(key))
        {
            newObjectiveMetrics.ObjectiveMetricCollection[key] = data;
            return;
        }

        newObjectiveMetrics.ObjectiveMetricCollection.Add(key, data);
    }

    public void RemoveObjectiveMetric(string key)
    {
        if (newObjectiveMetrics.ObjectiveMetricCollection.ContainsKey(key))
        {
            newObjectiveMetrics.ObjectiveMetricCollection.Remove(key);
            return;
        }
    }

    public void submitObjMetrics()
    {
        if(DataManager.Instance.GetSessionParticipant().Data == "")
        {
            UnityEngine.Debug.LogError("[OBJ METRIC ERROR] There was no session participant ID. This is probably an error in the code...");
            OnError.Invoke("[OBJ METRIC ERROR] There was no session participant ID. This is probably an error in the code...");
            return;
        }

        // does data directory exist?
        string dataPath = DataManager.Instance.GetSite().DataPath;
        if (!Directory.Exists(dataPath))
        {
            UnityEngine.Debug.LogError("[OBJ METRIC ERROR] Participant with that ID is not registered (no data directory). Please register them and come back.");
            OnError.Invoke("[OBJ METRIC ERROR] Participant with that ID is not registered (no data directory). Please register them and come back.");
            return;
        }

        //string participantListPath = DataManager.Instance.GetSite().GetAbsoluteParticipantListFilePath();
        //// does participant json file exist?
        //if (!File.Exists(participantListPath))
        //{
        //    Debug.LogError("[OBJ METRIC ERROR] Participant with that ID is not registered (no participant list json file). Please register them and come back.");
        //    return;
        //}

        //// participant list json file exists
        //string json = File.ReadAllText(participantListPath);
        //participantList = JsonConvert.DeserializeObject<ParticipantIdList>(json);

        //// check if participant is already registered
        //if (!participantList.ParticipantIDs.Contains(DataManager.Instance.GetSessionParticipant().Data))
        //{
        //    Debug.LogError("[SELECTION ERROR] Participant with that ID is not registered (participant ID not in list). Please register them and come back.");
        //    return;
        //}

        
        
        // DataManager.Instance.PrintScores();

        if(!CheckIfLastObjMetricsSameAsNew())
        {
            ObjectiveMetrics objectiveMetricsToSubmit = new ObjectiveMetrics();
            objectiveMetricsToSubmit.ParticipantId = DataManager.Instance.GetSessionParticipant().Data;
            objectiveMetricsToSubmit.ObjectiveMetricCollection = newObjectiveMetrics.ObjectiveMetricCollection;
            objectiveMetricsToSubmit.uuid = System.Guid.NewGuid().ToString();
            DataManager.Instance.SetObjMetrics(objectiveMetricsToSubmit);
            string objectiveMetricsToSubmitJson = JsonConvert.SerializeObject(objectiveMetricsToSubmit);
            string objMetricSavePath = DataManager.Instance.GetSite().DataPath + "/" + DataManager.Instance.GetSessionParticipant().Data + "/obj_metrics/" + "obj_metrics_" + objectiveMetricsToSubmit.uuid + ".json";
            File.WriteAllText(objMetricSavePath, objectiveMetricsToSubmitJson);
        } else
        {
            DataManager.Instance.SetObjMetrics(lastObjectiveMetrics);
        }

        onStartTherapy.Invoke();
    }

    public void UpdateParticipantID()
    {
        string id = DataManager.Instance.GetSessionParticipant().Data;
        instructionText.text = "Please Input Participant Details For: "+ id;
    }
}

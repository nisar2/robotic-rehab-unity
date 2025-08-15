using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using static Enums;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public string SessionId;

    [SerializeField] private SOSite site;
    [SerializeField] private SOString sessionParticipantId;

    [SerializeField] private ObjectiveMetrics objMetrics;

    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<DataManager>();
                    singletonObject.name = "Singleton";
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public SOSite GetSite()
    {
        return site;
    }

    public SOString GetSessionParticipant()
    {
        return sessionParticipantId;
    }

    public void SetObjMetrics(ObjectiveMetrics _objMetrics)
    {
        objMetrics = _objMetrics;
    }

    public void PrintScores()
    {
        foreach (KeyValuePair<string, float> entry in objMetrics.ObjectiveMetricCollection)
        {
            Debug.Log(entry.Key.ToString() + " : " + entry.Value.ToString());
        }
    }

    public void CreateSessionFolder()
    {
        SessionId = DateTime.Now.ToString("M-dd-yyyy--HH-mm-ss");
        string sessionFolderPath = site.DataPath + "/" + sessionParticipantId.Data + "/" + SessionId + "_"+ sessionParticipantId.Data;
        Debug.Log(sessionFolderPath);
        Directory.CreateDirectory(sessionFolderPath);

        string objectiveMetricsToSubmitJson = JsonConvert.SerializeObject(objMetrics);
        //Debug.Log(objectiveMetricsToSubmitJson);
        string objMetricSavePath = sessionFolderPath + "/obj_metrics.json";
        File.WriteAllText(objMetricSavePath, objectiveMetricsToSubmitJson);
    }
}

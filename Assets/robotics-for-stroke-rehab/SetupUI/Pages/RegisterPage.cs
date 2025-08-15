using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RegisterPage : MonoBehaviour
{
    //    [SerializeField] private SOString participantId;
    [SerializeField] private string participantId;
    [SerializeField] private Button registerButton;
    public UnityEvent OnUserRegistered = new UnityEvent();
    public UnityEvent<string> OnError = new UnityEvent<string>();

    //private ParticipantIdList participantList;

    private void OnEnable()
    {
        registerButton.onClick.AddListener(registerParticipantProtocol);
    }

    private void OnDisable()
    {
        registerButton.onClick.RemoveListener(registerParticipantProtocol);
    }

    public void UpdateParticipantId(string id)
    {
        participantId = id;
    }

    private void registerParticipantProtocol()
    {
        // participant id must be 8 characters
        if(participantId.Length != 3)
        {
            DataManager.Instance.GetSessionParticipant().Data = "";
            OnError.Invoke("[REGISTRATION ERROR] Participant ID needs to be 3 characters.");
            Debug.LogError("[REGISTRATION ERROR] Participant ID needs to be 3 characters.");
            return;
        }

        // if the data path does not exist, create it
        string dataPath = DataManager.Instance.GetSite().DataPath;
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        //string participantListPath = DataManager.Instance.GetSite().GetAbsoluteParticipantListFilePath();
        //// participant list json file does not exist, just add id, serialize and save
        //if (!File.Exists(participantListPath))
        //{
        //    participantList = new ParticipantIdList();
        //    register(participantId, participantListPath);
        //    return;
        //}

        //// participant list json file exists
        //string json = File.ReadAllText(participantListPath);
        //participantList = JsonConvert.DeserializeObject<ParticipantIdList>(json);

        // check if participant is already registered
        bool isRegistered = false;
        string[] subPaths = Directory.GetDirectories(DataManager.Instance.GetSite().DataPath);
        foreach(string path in subPaths)
        {
            
            string lastFolderName = Path.GetFileName(path);
            Debug.Log(lastFolderName);
            if (participantId == lastFolderName)
            {
                isRegistered = true;
                break;
            }
        }
        Debug.Log(isRegistered);
        if (isRegistered)
        {
            Debug.Log("HERE");
            DataManager.Instance.GetSessionParticipant().Data = "";
            Debug.LogError("[REGISTRATION ERROR] Participant ID is already registered.");
            OnError.Invoke("[REGISTRATION ERROR] Participant ID is already registered.");
            return;
        }

        // if participant is not already registered, register them
        register(participantId);
    }

    private void register(string participantIdToRegister)
    {
        //participantList.ParticipantIDs.Add(participantIdToRegister);
        //string participantListJson = JsonConvert.SerializeObject(participantList);
        //File.WriteAllText(participantListPath, participantListJson);
        Directory.CreateDirectory(DataManager.Instance.GetSite().GetAbsoluteParticipantFolderPath(participantIdToRegister));
        Directory.CreateDirectory(DataManager.Instance.GetSite().GetAbsoluteParticipantFolderPath(participantIdToRegister) + "/obj_metrics");
        // Directory.CreateDirectory(DataManager.Instance.GetSite().GetAbsoluteParticipantObjectiveMetricsFolderPath(participantIdToRegister));
        // Directory.CreateDirectory(DataManager.Instance.GetSite().GetAbsoluteParticipantSessionFolderPath(participantIdToRegister));
        DataManager.Instance.GetSessionParticipant().Data = participantIdToRegister;
        OnUserRegistered.Invoke();
    }
}
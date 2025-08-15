using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ParticipantSelectionPage : MonoBehaviour
{
    [SerializeField] private string participantId;
    [SerializeField] private Button selectionButton;

    [SerializeField] private UnityEvent onParticipantSelected = new UnityEvent();
    [SerializeField] private UnityEvent<string> OnError = new UnityEvent<string>();

    // private ParticipantIdList participantList;

    private void OnEnable()
    {
        selectionButton.onClick.AddListener(selectParticipant);
    }

    private void OnDisable()
    {
        selectionButton.onClick.RemoveListener(selectParticipant);
    }

    public void UpdateParticipantId(string id)
    {
        participantId = id;
    }

    private void selectParticipant()
    {
        Debug.Log("HERE");
        // participant id must be 8 characters
        if (participantId.Length != 3)
        {
            DataManager.Instance.GetSessionParticipant().Data = "";
            Debug.LogError("[SELECTION ERROR] Participant ID needs to be 3 characters.");
            OnError.Invoke("[SELECTION ERROR] Participant ID needs to be 3 characters.");
            return;
        }

        // does data directory exist?
        string dataPath = DataManager.Instance.GetSite().DataPath;
        if (!Directory.Exists(dataPath))
        {
            DataManager.Instance.GetSessionParticipant().Data = "";
            Debug.LogError("[SELECTION ERROR] Participant with that ID is not registered (no data directory). Please register them and come back.");
            OnError.Invoke("[SELECTION ERROR] Participant with that ID is not registered (no data directory). Please register them and come back.");
            return;
        }

        //string participantListPath = DataManager.Instance.GetSite().GetAbsoluteParticipantListFilePath();
        //// does participant json file exist?
        //if (!File.Exists(participantListPath))
        //{
        //    DataManager.Instance.GetSessionParticipant().Data = "";
        //    Debug.LogError("[SELECTION ERROR] Participant with that ID is not registered (no participant list json file). Please register them and come back.");
        //    return;
        //}

        //// participant list json file exists
        //string json = File.ReadAllText(participantListPath);
        // participantList = JsonConvert.DeserializeObject<ParticipantIdList>(json);

        // check if participant is already registered
        bool isRegistered = false;
        string[] subPaths = Directory.GetDirectories(DataManager.Instance.GetSite().DataPath);
        Debug.Log("HERE");
        foreach (string path in subPaths)
        {

            string lastFolderName = Path.GetFileName(path);
            Debug.Log(lastFolderName);
            if (participantId == lastFolderName)
            {
                isRegistered = true;
                break;
            }
        }
        if (!isRegistered)
        {
            DataManager.Instance.GetSessionParticipant().Data = "";
            Debug.LogError("[SELECTION ERROR] Participant with that ID is not registered (participant ID not in list). Please register them and come back.");
            OnError.Invoke("[SELECTION ERROR] Participant with that ID is not registered (participant ID not in list). Please register them and come back.");
            return;
        }

        DataManager.Instance.GetSessionParticipant().Data = participantId;
        
        onParticipantSelected.Invoke();
    }
}

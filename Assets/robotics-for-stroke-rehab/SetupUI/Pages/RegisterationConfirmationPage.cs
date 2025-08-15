using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterationConfirmationPage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI regConfirmationMessage;
    public void UpdateRegistrationConfirmationMessage()
    {
        regConfirmationMessage.text = DataManager.Instance.GetSessionParticipant().Data + " was registered.";
    }
}

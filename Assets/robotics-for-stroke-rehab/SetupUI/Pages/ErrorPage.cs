using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorPage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorMessage;
    public void UpdateErrorMessage(string message)
    {
        errorMessage.text = message;
    }
}

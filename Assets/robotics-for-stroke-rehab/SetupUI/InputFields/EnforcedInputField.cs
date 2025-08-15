using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public abstract class EnforcedInputField : MonoBehaviour
{
    [SerializeField] protected TMP_InputField input;

   private void OnEnable()
    {
        input.onValidateInput += ValidateData;
        input.onValueChanged.AddListener(UpdateData);
    }

    private void OnDisable()
    {
        input.onValidateInput -= ValidateData;
        input.onValueChanged.RemoveListener(UpdateData);
    }

    

    public abstract char ValidateData(string input, int charIndex, char addedChar);
    public abstract void UpdateData(string data);
}

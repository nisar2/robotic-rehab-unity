using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveMetricInputField : EnforcedInputField
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private float lowerLimit;
    [SerializeField] private float upperLimit;
    [SerializeField] private List<float> allowedValues = new List<float>();
    [SerializeField] private string key;
    [SerializeField] private string readableName;
    
    [SerializeField] UnityEvent<string, float> onObjectiveMetricInputUpdated = new UnityEvent<string, float>();
    [SerializeField] UnityEvent<string> onObjectiveMetricInputRemoved = new UnityEvent<string>();
    public void ManuallyUpdate(float? data)
    {
        Debug.Log(data.ToString());
        input.text = data.ToString();
    }

    public string GetKey() => key;
    public void SetKey(string toSet)
    {
        key = toSet;
    }

    public string GetReadableName() => readableName;
    public void SetReadableName(string toSet)
    {
        readableName = toSet;
        labelText.text = readableName;
    }


    public override char ValidateData(string input, int charIndex, char addedChar)
    {
        // Can't add spaces
        if (Char.IsWhiteSpace(addedChar))
        {
            return '\0';
        }

        // can only add a dash at the front
        if (addedChar == '-' & charIndex == 0 & lowerLimit < 0)
        {
            return '-';
        }

        // can only be a float
        string potentialInput = input + addedChar;
        float potentialInputFloat;
        bool isFloat = float.TryParse(potentialInput, out potentialInputFloat);
        if (!isFloat)
        {
            return '\0';
        }

        if (allowedValues.Count != 0)
        {
            bool isAllowed = false;
            foreach (float allowedFloat in allowedValues)
            {
                Debug.Log(allowedFloat);
                if (allowedFloat == potentialInputFloat)
                {
                    isAllowed = true;
                }
            }
            if (!isAllowed)
            {
                return '\0';
            }
        }

        // has to be in the limits
        if (potentialInputFloat < lowerLimit || potentialInputFloat > upperLimit)
        {
            return '\0';
        }

        // cant just spam 0's
        if(potentialInputFloat == 0 & potentialInput.Length > 1)
        {
            return '\0';
        }

        return addedChar;
    }
    public override void UpdateData(string data) 
    {
        Debug.Log(data);
        if(data == "")
        {
            onObjectiveMetricInputRemoved.Invoke(key);
            return;
        }
        float potentialInputFloat;
        bool isFloat = float.TryParse(data, out potentialInputFloat);
        if (isFloat)
        {
            onObjectiveMetricInputUpdated.Invoke(key, float.Parse(data));
        }
    }
}

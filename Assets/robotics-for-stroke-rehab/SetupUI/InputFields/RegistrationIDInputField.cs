using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RegistrationIDInputField : EnforcedInputField
{
    [SerializeField] private int maxLength;
    [SerializeField] UnityEvent<string> onRegistrationIDUpdated = new UnityEvent<string>();
    public override char ValidateData(string input, int charIndex, char addedChar)
    {
        if (Char.IsWhiteSpace(addedChar))
        {
            return '\0';
        }

        // has to be a certain amount of characters
        string potentialInput = input + addedChar;
        if (potentialInput.Length > maxLength)
        {
            return '\0';
        }

        // can only be an int
        int potentialInputInt;
        bool isInt = int.TryParse(potentialInput, out potentialInputInt);
        if (!isInt)
        {
            return '\0';
        }

        //Debug.Log($"{potentialInput}, {input}, {charIndex.ToString()}, {addedChar}");
        return addedChar;
    }

    public override void UpdateData(string data)
    {
        onRegistrationIDUpdated.Invoke(data);
    }
}

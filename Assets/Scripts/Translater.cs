using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class DebugString : MonoBehaviour 
{
    private LocalizedString _debugString;
    void Start()
    {
        Debug.Log(_debugString.GetLocalizedString());
    }

    public void ChangeLanguage()
    {
    
    }
}
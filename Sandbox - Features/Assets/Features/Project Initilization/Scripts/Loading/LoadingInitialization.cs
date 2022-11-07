using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingInitialization : MonoBehaviour
{
    #region Variables
    
    [SerializeField] private TextMeshProUGUI _loadText;
    
    #endregion


    #region Updates

    private void Awake()
    {
        SceneManager.loadText = _loadText;
    }

    #endregion
}

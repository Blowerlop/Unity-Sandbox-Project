using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SceneManager = Managers.SceneManager;

public class ff : MonoBehaviour
{
    private Button Button;

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    private void Start()
    {
        Button.onClick.AddListener(() => SceneManager.LoadSceneAsync(SceneManager.EScene.GridSystem));
    }
}

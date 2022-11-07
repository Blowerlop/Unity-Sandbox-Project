using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public static class SceneManager
    {
        #region Variables

        public enum EScene
        {
            Loading,
            GridSystem
        }

        public static TextMeshProUGUI loadText;

        #endregion

        #region Methods

        public static async void LoadSceneAsync(EScene sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (loadSceneMode == LoadSceneMode.Additive)
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);
            }
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != EScene.Loading.ToString())
            {
                AsyncOperation loadingScene =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(EScene.Loading.ToString());
                loadingScene.completed += (e) => LoadSceneAsync(sceneName, loadSceneMode);
            }
            else
            {
                AsyncOperation newScene =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);
                
                newScene.allowSceneActivation = false;
                do
                {
                    await Task.Delay(100);
                    float loadingPercentage = newScene.progress / 0.9f * 100.0f;
                    loadText.text = loadingPercentage + "%";
                    
                    
                    
                } while (newScene.progress < 0.9f);
                
                await Task.Delay(100);
                newScene.allowSceneActivation = true;
            }
        }
        #endregion
    }
}


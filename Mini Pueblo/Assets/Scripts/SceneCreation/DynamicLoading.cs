using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class DynamicLoading : MonoBehaviour
{
    AsyncOperationHandle<GameObject> prefabHandle;

    // Start is called before the first frame update
    void Start()
    {
        string prefabToLoad = PlayerPrefs.GetString("PrefabName", "");

        if (prefabToLoad == "")
        {
            SceneManager.LoadScene("Hub");
            return;
        }

        string externalCatalogPath = "";
        
        //Obtengo los datos de la escena tanto el catálogo como los ajustes
        if (Application.platform == RuntimePlatform.Android)
        {
            externalCatalogPath = System.IO.Path.Combine(Application.persistentDataPath, $"{prefabToLoad}/Android/StandaloneWindows64/catalog_1.0.json");
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            externalCatalogPath = System.IO.Path.Combine(Application.persistentDataPath, $"{prefabToLoad}/Windows/StandaloneWindows64/catalog_1.0.json");
        }

        Addressables.LoadContentCatalogAsync(externalCatalogPath).Completed += (catalogHandle) =>
        {
            if (catalogHandle.Status == AsyncOperationStatus.Succeeded)
            {
                //Addressables.InstantiateAsync($"Assets/Prefabs/{prefabToLoad}.prefab", new Vector3(0, 0, 0), Quaternion.identity);
                Addressables.LoadAssetAsync<GameObject>($"Assets/Prefabs/{prefabToLoad}.prefab").Completed += loadingFinished;
            }
            else
            {
                exitScene();
            }
        };
    }

    void loadingFinished(AsyncOperationHandle<GameObject> handle)
    {
        prefabHandle = handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = handle.Result;
            Instantiate(prefab);
        }
        else
        {
            exitScene();
        }
    }

    public void exitScene()
    {
        Addressables.ClearResourceLocators();
        if (prefabHandle.IsValid() == true)
        {
            Addressables.Release(prefabHandle);
        }
        SceneManager.LoadScene("Hub");
    }
}

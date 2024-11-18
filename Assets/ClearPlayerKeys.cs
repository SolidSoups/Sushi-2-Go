using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    private const string FirstLaunchKey = "NotFirstTime";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ClearFirstLaunchKey()
    {
        #if UNITY_EDITOR
        PlayerPrefs.DeleteKey(FirstLaunchKey);  // Clear only the "HasLaunchedBefore" key.
        PlayerPrefs.Save();  // Make sure changes are saved in the Editor.
        Debug.Log("First launch key cleared in Editor mode.");
        #endif
    }
}

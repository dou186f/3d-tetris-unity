using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StartupScene
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        const string sessionKey = "3DTetris.StartupSceneChecked";

        if (Application.isBatchMode || SessionState.GetBool(sessionKey, false))
        {
            return;
        }

        SessionState.SetBool(sessionKey, true);
        EditorApplication.delayCall += OpenSceneIfEmpty;
    }

    private static void OpenSceneIfEmpty()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.sceneCount != 1)
        {
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (string.IsNullOrEmpty(scene.path) && !scene.isDirty)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        }
    }
}

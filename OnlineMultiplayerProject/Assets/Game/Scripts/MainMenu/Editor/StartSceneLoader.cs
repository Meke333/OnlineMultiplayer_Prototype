using UnityEditor;
using UnityEditor.SceneManagement;


/// <summary>
/// Always load the first scene if you enter Play-Mode
/// </summary>
[InitializeOnLoad]
public static class StartSceneLoader
{
        static StartSceneLoader()
        {
            EditorApplication.playModeStateChanged += LoadStartupScene;
        }

        private static void LoadStartupScene(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingEditMode)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                //Loads the first scene from the build list (bootstrap)
                EditorSceneManager.LoadScene(0);
            }
        }
}

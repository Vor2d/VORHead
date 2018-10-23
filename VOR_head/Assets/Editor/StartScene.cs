using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class StartScene {

    [MenuItem("Edit/Play-Stop, But From Prelaunch Scene %0")]
    public static void PlayFromPrelaunchScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        //EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        //EditorApplication.OpenScene("Assets/whatever/YourPrepScene.unity");
        EditorSceneManager.OpenScene("Assets/Scenes/HMTS/HeadMInit.unity");
        EditorApplication.isPlaying = true;
    }
}

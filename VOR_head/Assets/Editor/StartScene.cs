using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class StartScene
{

    //private static Scene current_scene;

    [MenuItem("Edit/Play-Stop, But From Prelaunch Scene %0")]
    public static void PlayFromPrelaunchScene()
    {
        

        if (EditorApplication.isPlaying == true)
        {
            //SceneManager.LoadScene(current_scene.name);
            EditorApplication.isPlaying = false;
            return;
        }
        else
        {
            //current_scene = EditorSceneManager.GetActiveScene();
            //Debug.Log("current_scene " + current_scene.name);
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/StartScene/StartScene.unity");
            EditorApplication.isPlaying = true;
        }



    }

}

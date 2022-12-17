using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneIdentifier : MonoBehaviour
{
    private static SceneIdentifier instance;

    public static SceneIdentifier MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneIdentifier>();
            }

            return instance;
        }
    }
    public enum OpenScene
    {
        TitleScreen,

        DemoTest,

        Lab
    }

    public OpenScene openScene;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerManage : MonoBehaviour
{
    [SerializeField]
    private GameObject gameController;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameController.MyInstance == null)
        {
            Instantiate(gameController);
        }
    }
}

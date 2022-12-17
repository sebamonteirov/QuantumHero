using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateChildren : MonoBehaviour
{
    [SerializeField]
    private int needLevel;
    // Start is called before the first frame update
    void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (needLevel <= GameController.MyInstance.PlayerLevel)
        {
            //Debug.Log(needLevel + ">=" + GameController.MyInstance.PlayerLevel);
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}

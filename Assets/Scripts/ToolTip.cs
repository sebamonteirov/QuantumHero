using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (canvasGroup.alpha == 1)
            {
                canvasGroup.alpha = 0;
            } 
            /*else if (canvasGroup.alpha == 0)
            {
                canvasGroup.alpha = 1;
            }*/
        }
    }
}

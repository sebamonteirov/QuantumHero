using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderByLevel : MonoBehaviour
{
    [SerializeField]
    private int destroyLevel;
    // Update is called once per frame
    void Update()
    {
        if (Player.MyInstance.level >= destroyLevel)
        {
            Destroy(gameObject);
        }       
    }
}

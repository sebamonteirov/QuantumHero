using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScriptTilemap : MonoBehaviour
{
    [SerializeField]
    private Player.SoundWalk thisSound;

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            bool isPlayerMoving = col.GetComponentInParent<Player>().isMoving;

            if(isPlayerMoving)
            {
                //REPRODUCIR SONIDO DE PISADAS
                col.GetComponentInParent<Player>().playerSoundWalk = thisSound;
                //Debug.Log(col.GetComponentInParent<Player>().playerSoundWalk);
                //Debug.Log(thisSound);
            } 
        }
    }
}

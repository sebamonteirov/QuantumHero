using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class LabTable : MonoBehaviour
{
    [Tooltip("Dirección a la cual player tiene que mirar")]
    public Vector2 watch_to;
    [Tooltip("Dirección a la cual player está mirando")]
    public Vector2 watching_to;
    Animator playerAnim;
    //boolean para detctar si player está en la zona
    private bool inPos = false;
    // Start is called before the first frame update

    private void Start()
    {
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();
        //Debug.Log(playerAnim);
    }
    private void Update()
    {
        float x = playerAnim.GetFloat("posX");
        float y = playerAnim.GetFloat("posY");

        if (x != 0f || y != 0f)
        {
            watching_to = new Vector2(
                x,
                y
            );
        }

        if (inPos && watch_to == watching_to && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("cargando escena");
            //GameController.MyInstance.inLab = true;
            SceneManager.LoadScene("Lab");
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            inPos = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inPos = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extintor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("destroyThis");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Colision detectada");

        if (col.tag == "Flame")
        {
            Debug.Log("Colision con fuego");

            GameObjectStats stats = col.GetComponentInParent<GameObjectStats>();

            if (stats != null)
            {
                Debug.Log("Script de tags detectado");
            }

            stats.stat_hp -= 1;
        } 
        else if (col.tag == "Enemy")
        {
            Enemy scriptEnemy = col.GetComponentInParent<Enemy>();

            if(scriptEnemy != null)
            {
                Debug.Log(scriptEnemy + "script encontrado");

                //Para congelar al enemigo en un futuro
                //freeze();
                //O mejor programar la interaccion exclusivamente en script Enemy
            }
            else
            {
                Debug.Log("Script nulo");
            }
        }
    }


    private IEnumerator destroyThis()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}

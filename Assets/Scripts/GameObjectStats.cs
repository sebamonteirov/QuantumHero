using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectStats : MonoBehaviour
{
    public float stat_max_hp;
    public float stat_hp;

    private Enemy scriptEnemy;

    private void Awake()
    {
        stat_hp = stat_max_hp;
    }

    private void Start()
    {
        if (this.CompareTag("Enemy"))
        {
            scriptEnemy = GetComponent<Enemy>();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (this.CompareTag("Enemy"))
        {

            if (col.gameObject.tag == "Rock")
            {
                stat_hp -= 0.5f;
                //Cuando se cambia Enemy a Character hay que cambiar la linea por TakeDamage(0.5f);
            }
            else if (col.gameObject.tag == "Flame")
            {
                stat_hp -= 1;
                //Cuando se cambia Enemy a Character hay que cambiar la linea por TakeDamage(1f);
            }
        }
    }

}

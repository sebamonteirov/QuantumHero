using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolpePrefabScrip : MonoBehaviour
{
    public int Damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroyThis());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Enemy")
        {
            GameObjectStats stats = col.gameObject.GetComponent<GameObjectStats>();

            if (stats != null)
            {
                stats.stat_hp = stats.stat_hp - Damage;
            }

            Destroy(this.gameObject);
        }
        
    }

    private IEnumerator destroyThis()
    {
    	yield return new WaitForSeconds(0.5f);
    	Destroy(this.gameObject);
    }
}

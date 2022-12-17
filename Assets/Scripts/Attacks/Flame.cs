using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    GameObjectStats stats;
    Animator MyAnimator;
    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<GameObjectStats>();
        //Debug.Log(stats.stat_hp + "de" + stats.stat_max_hp);
        MyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.stat_hp <= 0)
        {
            StartCoroutine("Muriendo", 1f); //duración de la animación
        }
    }

    private IEnumerator Muriendo(float pAnim)
    {
        MyAnimator.SetTrigger("Die");
        yield return new WaitForSeconds(pAnim); //duración de la animación
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.isTrigger == false && collision.gameObject.CompareTag("Player"))
        {
            if (Player.MyInstance.isBurning == false)
            {
                Player.MyInstance.StartCoroutine("Stunned", 3);
                Player.MyInstance.TakeDamage(10);
                Player.MyInstance.isBurning = true;
                Player.MyInstance.StartCoroutine("ManageBurning", 5f);
            }
            else
            {
                Player.MyInstance.TakeDamage(10);
            }
        }
    }
}

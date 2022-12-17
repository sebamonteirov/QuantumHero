using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    [Tooltip("Velocidad de movimiento en unidades del mundo")]
    public float speed;

    GameObject targetGO;   // Recuperamos al objeto target
    public string targetTag;     //tag de target
    [SerializeField]
    public string summoner;//tag del invocador
    Rigidbody2D rb2d;    // Recuperamos el componente de cuerpo rígido
    [SerializeField]
    Vector3 target, dir; // Vectores para almacenar el objetivo y su dirección
    Player scriptPlayer;
    Animator playerAnim;

    void Start () 
    {
        rb2d = GetComponent<Rigidbody2D>();

        scriptPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();
        //Debug.Log(scriptPlayer);
        //Debug.Log(playerAnim);

        if (summoner != "Player")
        {
            targetGO = GameObject.FindGameObjectWithTag(targetTag);
            rb2d = GetComponent<Rigidbody2D>();

            // Recuperamos posición del jugador y la dirección normalizada
            if (targetGO != null)
            {
                target = targetGO.transform.position;
                dir = (target - transform.position).normalized;
            }
        } else if(summoner == "Player")
        {
            target = new Vector3(1, 1, 1);

            float x = playerAnim.GetFloat("posX");
            float y = playerAnim.GetFloat("posY");

            if (x != 0f || y != 0f)
            {
                dir = new Vector3(
                    x,
                    y,
                    transform.position.z
                );
            }
            

            //Debug.Log(target);
            //Debug.Log(dir);
        }
	}

    void FixedUpdate () {
        // Si hay un objetivo movemos la roca hacia su posición
        if (target != Vector3.zero) {
            rb2d.MovePosition(transform.position + (dir * speed) * Time.deltaTime);
        }
	}

    void OnTriggerEnter2D(Collider2D col){
        
        if (col.transform.tag == "Attack")
        {
            Destroy(gameObject); 
        } 
        else if(col.transform.tag == "Player")
        {
            Debug.Log("colision roca tag = player");
            if (summoner != "Player")
            {
                Player.MyInstance.StartCoroutine("Stunned", 2);
                Player.MyInstance.TakeDamage(8);
                Destroy(gameObject);
            }
        } 
        else if (col.transform.tag == "Enemy")
        {
            Debug.Log("colision roca tag = Enemy");
            if (summoner != "Enemy")
            {
                GameObjectStats stats = col.GetComponentInParent<GameObjectStats>();

                stats.stat_hp -= 1;

                Destroy(gameObject);
            }
        }
        else if(col.transform.tag == "TilemapCollider")
        {
            Destroy(gameObject);
            Debug.Log("colision roca");
        }
    }
}

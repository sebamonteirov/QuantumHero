using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    // Variables para gestionar el radio de visión, el de ataque y la velocidad
    public float visionRadius;
    public float attackRadius;
    public float speed;

    UnityEngine.Vector3 dir = new UnityEngine.Vector3(0, 0, 0);

    // Variables relacionadas con el ataque
    [Tooltip("Prefab de la roca que se disparará")]
    public GameObject rockPrefab;
    [Tooltip("Velocidad de ataque (segundos entre ataques)")]
    public float attackSpeed = 2f;
    bool attacking;
    [SerializeField]
    string attackTarget = "Player";

    ///--- Variables relacionadas con la vida
    [Tooltip("Puntos de vida")]
    public float maxHp;
    [Tooltip("Vida actual")]
    public float hp;
    ///Bool para identificar si la vida <= 0
    private bool isDead;
    // Variable para guardar al jugador
    GameObject player;

    // Variable para guardar la posición inicial y el objetivo
    UnityEngine.Vector3 initialPosition, target;

    // Animador y cuerpo cinemático con la rotación en Z congelada
    Animator anim;
    Rigidbody2D rb2d;
    GameObjectStats stats;

    [SerializeField]
    private bool isFreeze;

    void Start () {

        // Recuperamos al jugador gracias al Tag
        player = GameObject.FindGameObjectWithTag("Player");

        // Guardamos nuestra posición inicial
        initialPosition = transform.position;

        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        stats = GetComponent<GameObjectStats>();

        ///--- Iniciamos la vida
        hp = stats.stat_hp;
        maxHp = stats.stat_max_hp;

        hp = maxHp;
    }

    void Update () 
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        hp = stats.stat_hp;
        maxHp = stats.stat_max_hp;

        Attacked();

        // Por defecto nuestro target siempre será nuestra posición inicial
        target = transform.position;

        // Comprobamos un Raycast del enemigo hasta el jugador
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            player.transform.position - transform.position, 
            visionRadius, 
            1 << LayerMask.NameToLayer("Default") 
                // Poner el propio Enemy en una layer distinta a Default para evitar el raycast
                // También poner al objeto Attack y al Prefab Slash una Layer Attack 
                // Sino los detectará como entorno y se mueve atrás al hacer ataques
        );

        // Aquí podemos debugear el Raycast
        UnityEngine.Vector3 forward = transform.TransformDirection(player.transform.position - transform.position);
        Debug.DrawRay(transform.position, forward, Color.red);

        // Si el Raycast encuentra al jugador lo ponemos de target
        if (hit.collider != null) 
        {
            if (hit.collider.tag == "Player" && !isDead)
            {
                target = player.transform.position;
            }
        }

        // Calculamos la distancia y dirección actual hasta el target
        float distance = UnityEngine.Vector3.Distance(target, transform.position);
        dir = (target - transform.position).normalized;

        // Si es el enemigo y está en rango de ataque nos paramos y le atacamos
        if (target == player.transform.position && distance < attackRadius)
        {
            //26-08 enemigo no se mueve si player está en rango de ataque
            rb2d.velocity = new Vector2Int(0, 0);
            rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

            // Cambiamos la animación
            anim.SetFloat("movX", dir.x);
            anim.SetFloat("movY", dir.y);
            anim.Play("Enemy_Walk", -1, 0);  // Congela la animación de andar

            ///-- Empezamos a atacar (importante una Layer en ataque para evitar Raycast)
            if (!attacking) StartCoroutine(Attack(attackSpeed));
        }
        // En caso contrario nos movemos hacia él, target es player y distancia es menor a radio de vision
        else if(target == player.transform.position && distance < visionRadius)
        {
            //26-08
            rb2d.constraints = RigidbodyConstraints2D.None;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

            rb2d.MovePosition(transform.position + dir * speed * Time.deltaTime);

            // Al movernos establecemos la animación de movimiento
            anim.speed = 1;
            anim.SetFloat("movX", dir.x);
            anim.SetFloat("movY", dir.y);
            anim.SetBool("walking", true);

        } else{
            anim.SetBool("walking", false);
        }

        // Una última comprobación para evitar bugs forzando la posición inicial
        if (target == initialPosition && distance < 0.05f)
        {
            transform.position = initialPosition; 
            // Y cambiamos la animación de nuevo a Idle
            anim.SetBool("walking", false);
        }

        // Y un debug optativo con una línea hasta el target
        Debug.DrawLine(transform.position, target, Color.green);
    }

    // Podemos dibujar el radio de visión y ataque sobre la escena dibujando una esfera
    void OnDrawGizmosSelected() {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.DrawWireSphere(transform.position, attackRadius);

    }

    IEnumerator Attack(float seconds){
        attacking = true;  // Activamos la bandera
        // Si tenemos objetivo y el prefab es correcto creamos la roca
        if (target != initialPosition && rockPrefab != null) 
        {
            Rock RockScript = rockPrefab.GetComponent<Rock>();
            RockScript.targetTag = attackTarget;
            RockScript.summoner = this.tag;
            //Debug.Log(this.tag);
            Instantiate(rockPrefab, transform.position, transform.rotation);
            // Esperamos los segundos de turno antes de hacer otro ataque
            yield return new WaitForSeconds(seconds);
        }
        attacking = false; // Desactivamos la bandera
    }

    //Enemy recibiendo daño, por ahora solo se encarga de matarlo
    public void Attacked()
    {
        if (hp <= 0)
        {
            //debugear la vida para que no adquiera valores negativos
            stats.stat_hp = 0;

            isDead = true;

            anim.SetTrigger("isDead");

            target = transform.position;

            rb2d.velocity = new Vector2Int(0, 0);
            rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    ///---  Dibujamos las vidas del enemigo en una barra 
    void OnGUI() {
        // Guardamos la posición del enemigo en el mundo respecto a la cámara
        UnityEngine.Vector2 pos = Camera.main.WorldToScreenPoint (transform.position);

        // Dibujamos el cuadrado debajo del enemigo con el texto
        GUI.Box(
            new Rect(
                pos.x - 20,                   // posición x de la barra
                Screen.height - pos.y + 60,   // posición y de la barra
                40,                           // anchura de la barra    
                24                            // altura de la barra  
            ), hp + "/" + maxHp               // texto de la barra
        );
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Attack")
        {
            hp--;
        } 
        else if (col.tag == "Extintor")
        {
            StartCoroutine("ManageFreeze", 500000f);
        }
    }

    public IEnumerator ManageFreeze(float wait)
    {
        isFreeze = true;

        dir = UnityEngine.Vector3.zero;

        yield return new WaitForSeconds(wait);

        isFreeze = false;
    }

}
using System.Collections;
using UnityEngine;

public class EnemyBasedCharacter : Character
{
    // Variables para gestionar el radio de visión, el de ataque y la velocidad
    public float visionRadius;
    private float distance;
    public float attackRadius;

    // Variables relacionadas con el ataque
    [Tooltip("Prefab de la roca que se disparará")]
    public GameObject rockPrefab;

    [Tooltip("Velocidad de ataque (segundos entre ataques)")]
    public float attackSpeed = 2f;

    bool attacking;

    [SerializeField]
    string attackTarget = "Player";

    ///--- Variables relacionadas con la vida
    private GameObjectStats stats;
    [Tooltip("Puntos de vida")]
    public float maxHp;
    [Tooltip("Vida actual")]
    public float hp;
    ///Bool para identificar si la vida <= 0
    private bool isDead;
    // Variable para guardar al jugador
    GameObject player;

    // Variable para guardar la posición inicial y el objetivo
    Vector3 initialPosition, target;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Recuperamos al jugador gracias al Tag
        player = GameObject.FindGameObjectWithTag("Player");

        // Guardamos nuestra posición inicial
        initialPosition = transform.position;

        ///--- Iniciamos la vida
        maxHp = 3;
        hp = maxHp;//stats.stat_hp;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        ManageTarget();
    }

    private void ManageTarget()
    {
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
        Vector3 forward = transform.TransformDirection(player.transform.position - transform.position);
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
        distance = UnityEngine.Vector3.Distance(target, transform.position);
        direction = (target - transform.position).normalized;

        // Si es el enemigo y está en rango de ataque nos paramos y le atacamos
        if (target == player.transform.position && distance < attackRadius)
        {
            //direction = Vector2.zero;

            //26-08 enemigo no se mueve si player está en rango de ataque
            myRigidbody.velocity = new Vector2Int(0, 0);
            myRigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            ///-- Empezamos a atacar (importante una Layer en ataque para evitar Raycast)
            if (!attacking) StartCoroutine(Attack(attackSpeed));
        }
        // En caso contrario nos movemos hacia él, target es player y distancia es menor a radio de vision
        else if (target == player.transform.position && distance < visionRadius)
        {
            myRigidbody.constraints = RigidbodyConstraints2D.None;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Una última comprobación para evitar bugs forzando la posición inicial
        if (target == initialPosition && distance < 0.05f)
        {
            transform.position = initialPosition;
            //direction = Vector2.zero;
        }

        // Y un debug optativo con una línea hasta el target
        Debug.DrawLine(transform.position, target, Color.green);
    }

    // Podemos dibujar el radio de visión y ataque sobre la escena dibujando una esfera
    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.DrawWireSphere(transform.position, attackRadius);

    }

    IEnumerator Attack(float seconds)
    {
        isAttacking = true;
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

    private void ManageDead()
    {
        if (hp <= 0)
        {
            isDie = true;

            isBurning = false;

            isFreeze = false;
        }
    }
}

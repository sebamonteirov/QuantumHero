using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected Vector2 direction;

    protected Transform myTransform;

    protected Rigidbody2D myRigidbody;

    protected Animator myAnimator;

    protected AudioSource mySound;

    protected SpriteRenderer mySprite;

    protected GameObjectStats myStats;

    protected bool isAttacking = false, isDie = false, isStunned = false;

    protected Coroutine attackRoutine;

    public bool isFreeze, isBurning;

    [SerializeField]
    protected AudioClip AU_take_damage;

    public bool isMoving{
        get{
            //Si dir.x o dir.y no es igual a 0 quiere decir que me estoy moviendo
            return direction.x != 0 || direction.y != 0;
        }
    }

    public bool canMove = true;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySound = GetComponent<AudioSource>();
        myStats = GetComponent<GameObjectStats>();
        mySprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
   protected virtual void Update()
   {
        ManageCanMove();
        HandleLayers();
   }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        if (canMove)
        {
            //Muevo el personaje accediendo a su velocidad de rb2d, cambiandola por mi direccion
            //Aplico normalized para que no avance mas rapido en diagonal
            myRigidbody.constraints = RigidbodyConstraints2D.None;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            myRigidbody.velocity = direction.normalized * speed;
        }
        else if(!canMove)
        {
            //Congelo posición y rotacion de rb2d
            myRigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void HandleLayers()
    {

        //Si dir.x o dir.y no es igual a 0 quiere decir que me estoy moviendo
        if(isMoving)
        {
            //cambio a layer walk
            ActivateLayer("WalkLayer");
            //actualizo valores del animador
            //Si no me muevo, no se actualizan y por eso el jugador queda mirando en la ultima direccion
            myAnimator.SetFloat("posX", direction.x);
            myAnimator.SetFloat("posY", direction.y);

            //StopAttack();
        } else if(isAttacking)
        {
            ActivateLayer("AttackLayer");
        }
        else if (isFreeze)
        {
            ActivateLayer("IdleLayer");
        }
        else if (isDie)
        {
            ActivateLayer("DieLayer");
        }
        else if (isStunned)
        {
            ActivateLayer("StunnedLayer");
        }
        //Si no, aplico la layerIdle
        else
        {
            ActivateLayer("IdleLayer");
        }
    }

    public void ActivateLayer(string LayerName)
    {

        for (int i = 0; i < myAnimator.layerCount; i++)
        {
            //desactiva todas las layers del animator
            myAnimator.SetLayerWeight(i, 0);
        }
        //activa la que yo elija
        myAnimator.SetLayerWeight(myAnimator.GetLayerIndex(LayerName), 1);
    }

    public void StopAttack()
    {
        if(attackRoutine != null)
        {
            StopCoroutine("attackRoutine");
            isAttacking = false;
            myAnimator.SetBool("attack", isAttacking);
        }
    }

    private void ManageCanMove()
    {
        if (isAttacking || isDie || isStunned)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }
    }

    public void TakeDamage(float damage)
    {
        //Configurar y instanciar un gameObject que reproduzca un sonido
        /*GameObject objectAudio = new GameObject("objectAudio");
        objectAudio.tag = "damageAudio";
        AudioSource audioSource = objectAudio.AddComponent<AudioSource>() as AudioSource;
        ScriptDefaultSoundSource scriptDefaultSound = objectAudio.AddComponent<ScriptDefaultSoundSource>() as ScriptDefaultSoundSource;
        audioSource.clip = AU_take_damage;
        audioSource.playOnAwake = true;
        Instantiate(objectAudio);*/

        //si character no murió o está muerto
        if (!isDie)
        {
            //si no se está reproduciendo algun audio
            if (!mySound.isPlaying)
            {
                mySound.PlayOneShot(AU_take_damage);
            }

            //Quitar vida
            if (myStats != null)
            {
                myStats.stat_hp -= damage;
                Debug.Log("Restar 10 de vida a character (enemy)");
            }
            else
            {
                Player.MyInstance.MyHealth.MyCurrentValue -= damage;
                Debug.Log("Restar 10 de vida a player");
            }
        }
    }

    public IEnumerator ManageFreeze(float wait)
    {
        isFreeze = true;

        yield return new WaitForSeconds(wait);

        isFreeze = false;
    }

    public IEnumerator ManageBurning(float repeatFor)
    {
        Debug.Log("Cortina ManageBurning llamada");
        //le da a character un color naranjo
        mySprite.color = new Color(255, 65, 0, 255);

        if (isBurning == true)
        {
            for (int i = 0; i < repeatFor; i++)
            {
                //le da a character un color naranjo
                mySprite.color = new Color(255, 65, 0, 255);

                //si muere, el sprite color vuelve a blanco y termina el ciclo for
                if (isDie)
                {
                    mySprite.color = new Color(255, 255, 255, 255);

                    break;
                }

                //Quitar vida y le pone a character un color rojo
                if (myStats != null)
                {
                    mySprite.color = new Color(255, 0, 0, 255);
                    TakeDamage(1f);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    mySprite.color = new Color(255, 0, 0, 255);
                    TakeDamage(10f);
                    yield return new WaitForSeconds(1f);
                }

                //vuelve el color a naranjo
                mySprite.color = new Color(255, 65, 0, 255);

                yield return new WaitForSeconds(2f);
                Debug.Log("esperar " + 2 + " segundos");
                //StartCoroutine("ManageBurning", repeatFor);
            }

            mySprite.color = new Color(255, 255, 255, 255);
            isBurning = false;
        }
    }

    private IEnumerator Stunned(float stunnedTime)
    {
        StopAttack();
        isStunned = true;
        yield return new WaitForSeconds(stunnedTime);
        isStunned = false;
    }
}

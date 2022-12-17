using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private static Player instance;

    public static Player MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
            }

            return instance;
        }
    }

    //Este código crea el tipo de variable SoundWalk
    public enum SoundWalk
    {
        none,

        inGrass,

        inWoodIndoor
    }
    //Creo la variable de tipo SoundWalk para odentificar la superficie
    //si Player no se mueve = none
    public SoundWalk playerSoundWalk = SoundWalk.none;

    /// <summary>
    /// Variables de WalkSoundManager
    /// </summary>
    private GameObject walkSoundSource;
    public GameObject prefabWalkSoundSource;
    [SerializeField]
    private GameObject prefabWalkSoundSource1;
    [SerializeField]
    private AudioClip WalkGrass;
    [SerializeField]
    private AudioClip WalkWoodIndoor;
    private AudioClip audioClip;

    /// <summary>
    /// Variables de Sonidos de ataque
    /// </summary>
    [SerializeField]
    private AudioClip sonidoDeAtaqueEspada;

    /// <summary>
    /// Variables relacionadas con la cámara
    /// </summary>
    public float minX, maxX, minY, maxY;


    /// <summary>
    /// Variables relacionadas con la vida
    /// </summary>
    [SerializeField]
    private Stat health;

    public Stat MyHealth { get => health; set => health = value; }

    [SerializeField]
    private float initHealth = 100.0f;

    /// <summary>
    /// Variables Relacionadas con el mana/energia
    /// </summary>
    [SerializeField]
    private Stat mana;

    public Stat MyMana { get => mana; set => mana = value; }


    [SerializeField]
    private float initMana = 50.0f;

    public GameObject[] spellPrefab;
    //lugar del cual sale disparado el spell
    [SerializeField]
    private Transform[] exitPoints;

    [SerializeField]
    private int selectedSpell = 0;

    [SerializeField]
    private float animLength;

    public float MyAnimLength {
        get
        {
            switch (selectedSpell)
            {
                case 0: animLength = 0.5f; break; //espada normal
                case 1: animLength = 0.8f; break; //extintor
                case 2: animLength = 0.8f; break; //aqui estaba el extintor
                default: animLength = 1f; break; //default
            }

            return animLength;
        }
    }

    private int exitIndex;

    public int level = 0;

    //VARIABLES PARA ANIMACION DE TRANSICION PARA MUERTE

    //CONTROLA SI COMIENZA LA TRANSICION 
    bool start = false;
    //CONTROLA SI LA TRNSCN ES DE ENTRADA O SALIDA 
    bool isFadeIn = false;
    //OPACIDAD INICIAL
    float alpha = 0;
    //TIEMPO DE LA TRNSCN DE 1 SEGUNDO
    [SerializeField]
    float fadeTime = 0.1f;

    protected override void Start()
    {
        base.Start();
        MyHealth = GameObject.FindWithTag("Health").GetComponent<Stat>();
        MyMana = GameObject.FindWithTag("Mana").GetComponent<Stat>();
        MyHealth.Initialize(initHealth, initHealth);
        MyMana.Initialize(initMana, initMana);

        myTransform.position = GameController.MyInstance.PlayerLastPosition;
        level = GameController.MyInstance.PlayerLevel;
    }

    protected override void Update()
    {
        if (MyHealth.MyCurrentValue <= 0)
        {
            StartCoroutine("ManageDead");
        }
        selectedSpell = GameController.MyInstance.MySelectedSpell;
        GetInput();
        StartCoroutine("WalkSoundManager");
        base.Update();
    }

    private void GetInput()
    {
        direction = Vector2.zero;

        //---------SOLO DEBUGGING--------//
        if (Input.GetKeyDown(KeyCode.I))
        {
            TakeDamage(10);
            MyMana.MyCurrentValue -= 10;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            MyHealth.MyCurrentValue += 10;
            MyMana.MyCurrentValue += 10;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(MyHealth.MyCurrentValue);
            Debug.Log(MyMana.MyCurrentValue);
        }
        //----------SOLO DEBUGGING--------//

        if (canMove)
        {
            if (Input.GetKey(KeyCode.W))
            {
                exitIndex = 0;
                direction += Vector2.up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                exitIndex = 2;
                direction += Vector2.down;
            }
            if (Input.GetKey(KeyCode.A))
            {
                exitIndex = 3;
                direction += Vector2.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                exitIndex = 1;
                direction += Vector2.right;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            Debug.Log("Q presionada");
            GameController.MyInstance.MySelectedSpell++;
        }

        if (Input.GetKey(KeyCode.LeftShift) && MyMana.MyCurrentValue > 0 && isMoving)
        {
            //Debug.Log("Input.GetKey(KeyCode.LeftShift)");
            speed = 3.5f;
            MyMana.MyCurrentValue -= 1;
        }
        else
        {
            MyMana.MyCurrentValue += 1;
            speed = 2;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!isAttacking && !isMoving && !isStunned)
            {
                attackRoutine = StartCoroutine(Attack(selectedSpell, MyAnimLength));
            }
        }
    }

    private IEnumerator ManageDead()
    {
        isDie = true;

        isBurning = false;

        isFreeze = false;

        FadeIn();

        yield return new WaitForSeconds(3);

        transform.position = new Vector3(-23, 12.43f, 0);

        minX = -20;
        maxX = -20;
        minY = 13;
        maxY = 15;

        MyHealth.Initialize(initHealth, initHealth);
        MyMana.Initialize(initMana, initMana);

        FadeOut();

        isDie = false;
    }

    private IEnumerator Attack(int pSpellIndex, float pAnimDuration)
    {
        direction = Vector2.zero;

        isAttacking = true;

        myAnimator.SetBool("attack", isAttacking);

        CastSpell(pSpellIndex);

        yield return new WaitForSeconds(pAnimDuration); //0.5
        Debug.Log("attack done");
        StopAttack();
    }

    public void CastSpell(int pSpellIndex)
    {
        GameObject attack = spellPrefab[pSpellIndex];
        AudioSource audioSource = attack.GetComponent<AudioSource>();

        if (audioSource != null)
        {
            if (pSpellIndex == 0)
            {
                audioSource.clip = sonidoDeAtaqueEspada;
            }
        }
        
        Instantiate(spellPrefab[pSpellIndex], exitPoints[exitIndex].position, exitPoints[exitIndex].rotation);

        //Configuración para roca
        /*if (pSpellIndex == 1)
        {
            Rock RockScript = spellPrefab[pSpellIndex].GetComponent<Rock>();
            
            RockScript.summoner = this.tag;

            GameObject Enemy = GameObject.FindGameObjectWithTag("Enemy");

            if(Enemy != null)
            {
                RockScript.targetTag = "Enemy";
            } else
            {
                RockScript.targetTag = null;
            }
        }*/
    }

    private IEnumerator WalkSoundManager()
    {
        
        if(prefabWalkSoundSource == null && prefabWalkSoundSource1 != null)
        {
            prefabWalkSoundSource = prefabWalkSoundSource1;
        }

        if (!isMoving)
        {
            playerSoundWalk = SoundWalk.none;
            //Debug.Log("movingn't XDD");
        } 
        else if (isMoving && playerSoundWalk != SoundWalk.none && canMove) //si la variable encargada de identificar la superficie es diferente a none
        {
            //busco el go encargado de reproducir el sonido
            walkSoundSource = GameObject.FindWithTag("walkSoundSource");
            //sino no existe
            if(walkSoundSource == null)
            {
                //lo declaro como igual al prefab que luego instanciaré
                walkSoundSource = prefabWalkSoundSource;

                //declaro la variable audioSource para acceder al componente
                AudioSource audioSource = walkSoundSource.GetComponent<AudioSource>();

                //Selecciona audio segun superficie
                if(playerSoundWalk == SoundWalk.inGrass)
                {
                    audioClip = WalkGrass;
                }
                else if(playerSoundWalk == SoundWalk.inWoodIndoor)
                {
                    audioClip = WalkWoodIndoor;
                }
                //El clip de audio source es igual al seleccionado
                audioSource.clip = audioClip;

                Instantiate(walkSoundSource);

                //espero a que el sonido termine
                yield return new WaitForSeconds(0);
            }
        }
    }

    /// <summary>
    /// Clases para transicion a pantalla en negro NO TOCAR
    /// </summary>
    void OnGUI()
    {

        //si no empieza sale del evento
        if (!start) return;

        //Si comienza entonces crea un color con opacidad 0
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);

        //creamos textura temporal para rellenar la pantalla

        Texture2D tex;
        tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.black);
        tex.Apply();

        //se dibuja la textura 
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex);

        //se controla la transpariencia 
        if (isFadeIn)
        {
            //Si es la de aparecer le sumamos opacidad
            alpha = Mathf.Lerp(alpha, 1.1f, fadeTime * Time.deltaTime);
        }
        else
        {
            //Si es la de desaparecer le restamos opacidad
            alpha = Mathf.Lerp(alpha, -0.1f, fadeTime * Time.deltaTime);
            //si opacidad = 0 se desactiva la transicion
            if (alpha < 0) start = false;
        }


    }

    void FadeIn()
    {
        start = true;
        isFadeIn = true;
    }

    void FadeOut()
    {
        isFadeIn = false;
    }
}

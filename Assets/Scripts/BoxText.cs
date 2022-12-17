using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxText : MonoBehaviour
{
    public enum TypeOfText
    {
        Dialogue,

        Tooltip,

        Dialogue1Text,

        TeachMoment
    }

    [SerializeField]
    private TypeOfText typeOfText;

    /// <summary>
    /// RECUPERA EL GAMEOBJECT EN ESCENA y sus componentes
    /// </summary>

    //recuperar el go tooltip
    private GameObject TextBox;
    //crear la variable para guardar la imagen
    private Image image;
    // guardar el canvas group
    private CanvasGroup canvasGroup;
    //guardar el component text del hijo
    private Text componentText;

    //sprite para uso varible (como marco para dialogue e imagen para teachMoment)
    [SerializeField]
    private Sprite SelectedSprite;

    //crear la variable para guardar el lvl de player
    private int playerLevel = 0;
    //nivel necesario para que aparezca el teachmoment
    [SerializeField]
    private int needLevel = 0;

    //texto del tooltip o de Dialogue1Text
    [SerializeField]
    [Tooltip("texto del tooltip o de Dialogue1Text")]
    private string string_tooltip = "";
    //Array que guarda los dialogos en orden
    [SerializeField]
    private string[] dialogues;

    /// <summary>
    /// Booleans
    /// </summary>
    [SerializeField]
    private bool IncreaseLevel = false;
    [SerializeField]
    private bool inPos = false;

    /// <summary>
    /// variables para que el jugador tenga que mirar a una direccion para activar el texto
    /// </summary>
    /// [Tooltip("Dirección a la cual player tiene que mirar")]
    public Vector2 watch_to;
    [Tooltip("Dirección a la cual player está mirando")]
    public Vector2 watching_to;
    Player scriptPlayer;
    Animator playerAnim;


    //Array que define en qué niveles se va a sumar 1 al nivel de player
    [SerializeField]
    private int[] ArrayIncreaseLevel;

    //texto q se va a mostrar en pantalla
    private string text = "";

    public string MyText
    {
        get
        {
            if (typeOfText == TypeOfText.Dialogue)
            {
                text = dialogues[playerLevel];
            }
            else if (typeOfText == TypeOfText.Tooltip || typeOfText == TypeOfText.Dialogue1Text || typeOfText == TypeOfText.TeachMoment)
            {
                text = string_tooltip;
            }

            return text;
        }
    }

    void Start()
    {
        //quitar el sprite
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        //Recupera el elemento del canvas segun sea dialogo o tooltip y se guarda en la variable TextBox
        if (typeOfText == TypeOfText.Tooltip)
        {
            TextBox = GameObject.FindWithTag("ToolTip");
        } 
        else if(typeOfText == TypeOfText.Dialogue || typeOfText == TypeOfText.Dialogue1Text)
        {
            TextBox = GameObject.FindWithTag("Dialogue");
        }
        else if (typeOfText == TypeOfText.TeachMoment)
        {
            TextBox = GameObject.FindWithTag("TeachMoment");
        }

        canvasGroup = TextBox.GetComponent<CanvasGroup>();

        componentText = TextBox.GetComponentInChildren<Text>();

        /*if (typeOfText == TypeOfText.TeachMoment)
        {
            image = TextBox.gameObject.transform.GetChild(1).GetComponent<Image>();

            image.sprite = SelectedSprite;
        }*/

        
        

        //recuperar el level de player
        playerLevel = Player.MyInstance.level;
        //opacidad de la imagen es 0
        canvasGroup.alpha = 0;
        //recuperar componente animator de player
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inPos && watching_to == watch_to)
        {
            ManageView(null);
        }

        playerLevel = Player.MyInstance.level;

        float x = playerAnim.GetFloat("posX");
        float y = playerAnim.GetFloat("posY");

        if (x != 0f || y != 0f)
        {
            watching_to = new Vector2(
                x,
                y
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            inPos = true;
        }

        ManageView(col);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            inPos = false;
        }
    }

    private void ManageView(Collider2D col)
    {
        //Define la imagen que va a aparecer junto al texto
        try
        {
            image = TextBox.gameObject.transform.GetChild(1).GetComponent<Image>();

            image.sprite = SelectedSprite;
        }
        catch (System.Exception)
        {

            image = null;

            SelectedSprite = null;

            Debug.Log("No existe componente imagen en TextBox");
        }


        //Divide los tipos de texto según si es necesario o no presionar E para que salte, 
        //si collider es null ent11 aparece por presionar E sino aparece automaticamente por entrar en contacto
        if (col == null)
        {
            if (typeOfText == TypeOfText.Dialogue)
            {
                if (MyText != null)
                {
                    //aparece
                    canvasGroup.alpha = 1;

                    //Define el texto del componente del canvas
                    componentText.text = MyText;

                    //se comprueba si tiene que subir de nivel
                    foreach (var i in ArrayIncreaseLevel)
                    {
                        //ArrayIncreaseLevel consta de los index de dialogues(strings) en los que tiene que subir el nivel
                        if (MyText == dialogues[i])
                        {
                            Player.MyInstance.level++;
                        }
                    }
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (typeOfText == TypeOfText.Dialogue1Text)
            {
                if (MyText != null)
                {
                    //aparece
                    canvasGroup.alpha = 1;
                    //determino el texto del componente en canvas
                    componentText.text = MyText;
                }
            }
        }
        else //si col != null
        {
            //Condiciones para que salte el tooltip
            if (typeOfText == TypeOfText.Tooltip && col.tag == "Player" && !GameController.MyInstance.tooltipDisplayed.Contains(this.gameObject.name))
            {
                //aparece el tooltip
                canvasGroup.alpha = 1;

                componentText.text = MyText;

                this.gameObject.SetActive(false);

                if (IncreaseLevel)
                {
                    Player.MyInstance.level++;
                }

                GameController.MyInstance.tooltipDisplayed.Add(this.gameObject.name);
                //Debug.Log(this.gameObject.name);
            }
            //Condiciones para que salte TeachMoment
            if (typeOfText == TypeOfText.TeachMoment && col.tag == "Player" && playerLevel == needLevel)
            {
                //aparece el tooltip
                canvasGroup.alpha = 1;

                componentText.text = MyText;

                //se desactiva el gameobject porque los de este tipo solo tienen una aparicion en la historia
                this.gameObject.SetActive(false);

                if (IncreaseLevel)
                {
                    Player.MyInstance.level++;
                }
            }
        }
    }
}

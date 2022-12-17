using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    private static GameController instance;

    public static GameController MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameController>();
            }

            return instance;
        }
    }

    private GameObject player;

    public Vector3 PlayerLastPosition;

    public int PlayerLevel = 0;

    public int openBags;
    public int openWeaponBags;
    public int healthPotionCount;

    [SerializeField]
    public int MySelectedSpell;

    public List<string> weaponsUnlock = new List<string>();

    [SerializeField]
    private SceneIdentifier.OpenScene openScene;

    public List<string> tooltipDisplayed = new List<string>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        //Recupera el objeto Player
        try
        {
            player = GameObject.FindWithTag("Player");
        }
        catch (System.Exception)
        {
            player = null;
        }
        
        //Determinar la primera posicion de player y nivel de player
        if (player != null)
        {
            PlayerLastPosition = player.GetComponent<Transform>().position;
            PlayerLevel = Player.MyInstance.level;
        }

        //Determinar la scene abierta
        try
        {
            openScene = SceneIdentifier.MyInstance.openScene;
        }
        catch (System.Exception)
        {
            Debug.Log("escena sin identificador");
        }


        GetInput();

        DebugOpenBags();
        DebugSelectedSpell();

        Lab();
        TitleScreen();
    }

    private void GetInput()
    {
        //Pausa
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
        //Salir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Salir();
        }
    }

    void DebugOpenBags()
    {
        //debuguear la cantidad de bolsas posible
        if (openBags >= 3)
        {
            openBags = 2;
        }
    }
    public void Salir()
    {
        Application.Quit();
    }

    private void Pause()
    {
        CanvasGroup blackImage = GameObject.FindGameObjectWithTag("BlackImage").GetComponent<CanvasGroup>();
        CanvasGroup pauseCanvas = GameObject.FindGameObjectWithTag("PauseCanvas").GetComponent<CanvasGroup>();


        float vTimeScale = Time.timeScale;
        Time.timeScale = (vTimeScale == 1) ? 0 : 1;

        blackImage.alpha = (vTimeScale == 1) ? 0.3f : 0;
        pauseCanvas.alpha = (vTimeScale == 1) ? 1 : 0;
    }
    void DebugSelectedSpell()
    {
        if (Player.MyInstance != null)
        {
            if (MySelectedSpell >= Player.MyInstance.spellPrefab.Length || MySelectedSpell <= -1)
            {
                MySelectedSpell = 0;
            }
            if (MySelectedSpell == 1 && !weaponsUnlock.Contains("Extintor"))
            {
                MySelectedSpell++;
            }
        }
        
    }

    private void Lab()
    {
        if (openScene == SceneIdentifier.OpenScene.Lab)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene("demo-test");
            }
        }
    }

    private void TitleScreen()
    {
        if (openScene == SceneIdentifier.OpenScene.TitleScreen)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("return presionado");
                SceneManager.LoadScene("demo-test");
            }
        }
    } 
}

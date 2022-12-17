using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderChangeScene : MonoBehaviour
{
    public string sceneName;

    [SerializeField]
    private int needLevel;

    SpriteRenderer spriteRenderer;

    //VARIABLES PARA ANIMACION DE TRANSICION 

    //CONTROLA SI COMIENZA LA TRANSICION 
    bool start = false;
    //CONTROLA SI LA TRNSCN ES DE ENTRADA O SALIDA 
    bool isFadeIn = false;
    //OPACIDAD INICIAL
    float alpha = 0;
    //TIEMPO DE LA TRNSCN DE 1 SEGUNDO
    [SerializeField]
    float fadeTime = 0.1f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private IEnumerator OnTriggerEnter2D(Collider2D col)
    {
        if (GameController.MyInstance.PlayerLevel >= needLevel)
        {
            Debug.Log("cargando" + sceneName);
            FadeIn();
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(sceneName);
            FadeOut();
        }
    }


    ///OnGUI es para dibujar por encima de la pantalla
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

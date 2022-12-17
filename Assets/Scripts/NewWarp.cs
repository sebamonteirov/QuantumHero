using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class NewWarp : MonoBehaviour
{
	public GameObject target;

    public Player scripPlayer;

    //Limites de camara del mapa target
    //[minX, maxX, minY, maxY]
    public float[] infoTarget;

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

	void Awake(){

        //esto es para comprobar que target tiene valor
        Assert.IsNotNull(target);

        //esconde el sprite
		GetComponent<SpriteRenderer>().enabled = false;
		transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            other.GetComponent<Animator>().enabled = false;
            other.GetComponent<Player>().enabled = false;
            Rigidbody2D rb2d = other.GetComponent<Rigidbody2D>();
            rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            FadeIn();

            yield return new WaitForSeconds(fadeTime);


            other.transform.position = new Vector3(target.transform.GetChild(0).transform.position.x, target.transform.GetChild(0).transform.position.y, 0);
            other.GetComponentInParent<Player>().minX = infoTarget[0];
            other.GetComponentInParent<Player>().maxX = infoTarget[1];
            other.GetComponentInParent<Player>().minY = infoTarget[2];
            other.GetComponentInParent<Player>().maxY = infoTarget[3];
            

            //esto es para mover rápido la cámara (proximamente cambiar limites del mapa)
            Camera.main.GetComponent<ScriptCamara1>().SetBound();


            FadeOut();
            other.GetComponent<Animator>().enabled = true;
            other.GetComponent<Player>().enabled = true;
            rb2d.constraints = RigidbodyConstraints2D.None;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }




    //OnGUI es para dibujar por encima de la pantalla
    void OnGUI() {

        //si no empieza sale del evento
        if (!start) return;

        //se dibuja la textura 
        CanvasGroup blackImage = GameObject.FindGameObjectWithTag("BlackImage").GetComponent<CanvasGroup>();

        blackImage.alpha = alpha;

        //se controla la transpariencia 
        if (isFadeIn){
            //Si es la de aparecer le sumamos opacidad
            alpha = Mathf.Lerp (alpha, 1f, fadeTime * Time.deltaTime);
        } 
        else{
            //Si es la de desaparecer le restamos opacidad
            alpha = Mathf.Lerp (alpha, 0, fadeTime * Time.deltaTime);
            //si opacidad = 0 se desactiva la transicion
            if (alpha < 0) start = false;
        }


    }

    void FadeIn (){
        start = true;
        isFadeIn = true;
    }

    void FadeOut (){
        isFadeIn = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cup : MonoBehaviour
{
    private static InventoryScript instance;

    public static InventoryScript MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryScript>();
            }

            return instance;
        }
    }
    //Gameobjects en cup
    public List<GameObject> inCup = new List<GameObject>();
    
    //Gameobjects que existen y se pueden mezclar en cup
    public List<string> materials = new List<string>();

    //Objetos para instanciar
    public List<GameObject> forInstantiate = new List<GameObject>();
    //Materiales desbloqueados
    public List<GameObject> materialesUnlock = new List<GameObject>();
    //Objetos creado
    public List<string> objetosCreados = new List<string>();

    //crear una variable booleana para comprobar si cup contiene un material
    //el nombre de var tiene que ser is"nombre_del_objeto"

    //materiales de prueba
    public bool isRed = false;
    public bool isYellow = false;
    public bool isWoodenStick = false;
    public bool isElastic = false;

    //primera mision
    public bool isWater;
    public bool isBicarbonate;
    public bool isLemon;

    //Index de los objetos inCup

    //materiales de prueba
    public int redIndex;
    public int yellowIndex;
    public int woodenStickIndex;
    public int elasticIndex;

    //primera mision
    public int waterIndex;
    public int bicarbonateIndex;
    public int lemonIndex;


    void OnTriggerEnter2D(Collider2D col)
    {
        
        if(!inCup.Contains(col.gameObject))
        {
            //si el tag es material
            if(col.CompareTag("Material"))
            {
                //si en la lista hay un item nulo
                if(inCup.Contains(null))
                {
                    //busco cual es el index item nulo
                    for (int i = 0; i < inCup.Count; i++)
                    {
                        if(inCup[i] == null)
                        {
                            inCup[i] = col.gameObject;
                        }
                    }
                } else //de lo contrario solo agrego el item y alargo la lista
                {
                    inCup.Add(col.gameObject);
                }
            }
        }
            
    }

    void OnTriggerExit2D(Collider2D col)
    {
        //remover el item
        if(inCup.Contains(col.gameObject))
        {
            inCup.Remove(col.gameObject);
        }
    }

    public void Mix()
    {
        Debug.Log("boton presionado");

        foreach (var material in materials)
        {
            for (int i = 0; i < inCup.Count; i++)
            {
                if (inCup[i].name == material)
                {
                    Debug.Log("Cup contiene  " + material);

                    switch (material)
                    {
                        //SE DEBEN AGREGAR TODOS LOS MATERIALES Y ASOCIARLOS CON SU VARIABLE BOOLEAN E INDEX
                        
                        //test
                        case "red": isRed = true; redIndex = i; break;
                        case "yellow": isYellow = true; yellowIndex = i; break;
                        //primera mision
                        case "water": isWater = true; waterIndex = i; break;
                        case "bicarbonate": isBicarbonate = true; bicarbonateIndex = i; break;
                        case "lemon": isLemon = true; lemonIndex = i; break;
                    }
                }
            }
        }
        StartCoroutine(CheckCombination());
    }

    public IEnumerator CheckCombination()
    {
        //Test
        if(isRed && isYellow)
        {
            for (int i = 0; i < inCup.Count; i++)
            {
                if(inCup[i].name == "red")
                {
                    Destroy(inCup[i]);
                    isRed = false;
                }
            }
            for (int i = 0; i < inCup.Count; i++)
            {
                if (inCup[i].name == "yellow")
                {
                    Destroy(inCup[i]);
                    isYellow = false;
                }
            }
            yield return new WaitForSeconds(0.5f);

            Instantiate(forInstantiate[0], transform.position, transform.rotation);

        }

        //Tutorial Resortera
        if(isWoodenStick && isElastic)
        {
            for (int i = 0; i < inCup.Count; i++)
            {
                if(inCup[i].name == "Wooden Stick")
                {
                    Destroy(inCup[i]);
                    isWoodenStick = false;
                }
                if(inCup[i].name == "Elastic")
                {
                    Destroy(inCup[i]);
                    isElastic = false;
                }
            }
            yield return new WaitForSeconds(0.5f);

            Instantiate(forInstantiate[1], transform.position, transform.rotation);

        }
        //primera misión - EXTINTOR
        if (isWater && isBicarbonate && isLemon)
        {
            for (int i = 0; i < inCup.Count; i++)
            {
                if (inCup[i].name == "water")
                {
                    Destroy(inCup[i]);
                    isWater = false;
                }
            }
            for (int i = 0; i < inCup.Count; i++)
            {
                if (inCup[i].name == "bicarbonate")
                {
                    Destroy(inCup[i]);
                    isBicarbonate = false;
                }
            }
            for (int i = 0; i < inCup.Count; i++)
            {
                if (inCup[i].name == "lemon")
                {
                    Destroy(inCup[i]);
                    isLemon = false;
                }
            }
            yield return new WaitForSeconds(0.5f);

            //Si es la primera vez que se crea e extintor
            if (!objetosCreados.Contains("Extintor"))
            {
                //se agrega a objetos creados
                objetosCreados.Add("Extintor");
                //y a armas desbloqueadas
                try
                {
                    GameController.MyInstance.weaponsUnlock.Add("Extintor");
                }
                catch (System.Exception)
                {
                    Debug.LogError("No existe game controller");
                }
                
                if (GameController.MyInstance.PlayerLevel == 2)
                {
                    GameController.MyInstance.PlayerLevel++;
                }
                
            }

            Instantiate(forInstantiate[2], transform.position, transform.rotation);

        }
    }
}

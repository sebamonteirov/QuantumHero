using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLab : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabButtonMaterial;

    public bool createButon;

    public GameObject[] materiales, unlockMateriales;

    public int instantiate;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var material in unlockMateriales)
        {
            GameObject buttonMaterial = Instantiate(prefabButtonMaterial);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (instantiate >= 3)
        {
            instantiate = 0;
        }
    }

    public void InstantiateMaterial(GameObject material)
    {
        Vector3 pos = Vector3.zero;

        if (material != null)
        {
            switch (instantiate)
            {
                case 0: pos = new Vector3(-16, 0, -2); instantiate++; break;
                case 1: pos = new Vector3(-13, 0, -2); instantiate++; break;
                case 2: pos = new Vector3(-10, 0, -2); instantiate++; break;
                default:
                    break;
            }

            GameObject instanciado = Instantiate(material, pos, Quaternion.identity);
            instanciado.name = material.name;
        }
    }
}

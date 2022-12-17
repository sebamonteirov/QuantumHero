using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventoryScript : MonoBehaviour
{
    private static WeaponInventoryScript instance;

    public static WeaponInventoryScript MyInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<WeaponInventoryScript>();
            }

            return instance;
        }
    }

    private List <Bag> bags = new List <Bag>(); 

    [SerializeField]
    private BagButton[] bagButtons;

    [SerializeField]
    private Item[] items;

    public bool CanAddBag
    {
        get{return bags.Count < 3;}
    }

    private void Awake()
    {
        BagsAwake();

        for (int i = 0; i < GameController.MyInstance.healthPotionCount; i++)
        {
            HealthPotion potion = (HealthPotion)(Instantiate(items[1]));
            AddItem(potion);
        }
    }

    private void BagsAwake() 
    {
        Bag bag2 = (Bag)(Instantiate(items[0]));
        bag2.Initialize(4);
        bag2.Use();

        OpenClose();

        GameController.MyInstance.openWeaponBags = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            OpenClose();
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            HealthPotion potion = (HealthPotion)(Instantiate(items[1]));
            AddItem(potion);
        }
    }
    
    public void AddBag(Bag bag)
    {
        GameController.MyInstance.openWeaponBags++;

        foreach (BagButton bagButton in bagButtons)
        {
            if (bagButton.MyBag == null)
            {
                bagButton.MyBag = bag;
                bags.Add(bag);
                break;
            }
        }
    }

    public bool AddItem(Item item)
    {
        foreach (Bag bag in bags)
        {
            //si el item agregado es una pocion de vida le sumo uno al contador de pociones para guardar la cantidad
            if(item = items[1])
            {
                GameController.MyInstance.healthPotionCount++;
            }
            if(bag.MyBagScript.AddItem(item))
            {
                return false;
            }
        }

        return false;
    }

    public void OpenClose()
    {
        bool closedBag = bags.Find(x => !x.MyBagScript.IsOpen);
        //if clossed bag == true, entonces abre todas las bags cerradas
        //if clossed bag == false, entonces cierra todas las bags

        foreach (Bag bag in bags)
        {
            if (bag.MyBagScript.IsOpen != closedBag)
            {
                bag.MyBagScript.OpenClose();
            }
        }
    }
}

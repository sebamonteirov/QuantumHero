using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Bag", menuName = "Items/Bag", order = 1)]
public class Bag : Item, IUseable
{
    private int slots;

    [SerializeField]
    private GameObject bagPrefab;

   public BagScript MyBagScript{ get; set; }

    public int Slots 
    { 
        get
        {
            return slots;
        }
    }

    public Sprite MyIcon => throw new System.NotImplementedException();

    public void Initialize(int slots)
    {
        this.slots = slots;
    }

    public void Use()
    {
        if(InventoryScript.MyInstance.CanAddBag)
        {
            Remove();
            MyBagScript = Instantiate(bagPrefab,InventoryScript.MyInstance.transform).GetComponent<BagScript>();
            MyBagScript.AddSlots(slots);

            InventoryScript.MyInstance.AddBag(this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedWeaponHUD : MonoBehaviour
{
    public Image MyImage;

    public Sprite sword, extintor, weapon2;

    private Sprite SelectedSprite;

    private int selectedSpell;
    // Start is called before the first frame update
    void Start()
    {
        MyImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        selectedSpell = GameController.MyInstance.MySelectedSpell;

        MyImage.sprite = SelectedSprite;

        switch (selectedSpell)
        {
            case 0: SelectedSprite = sword; break;
            case 1: SelectedSprite = extintor; break;
            default:
                break;
        }
    }
}
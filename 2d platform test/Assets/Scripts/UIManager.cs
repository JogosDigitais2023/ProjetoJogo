using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image imageMage;
    [SerializeField] private Image imageSlinger;
    [SerializeField] private Image imageWarrior;
    [SerializeField] private PlayerController player;

    private void SetSelecterCharacter(PlayerController player)
    {
        // switch (characterState)
        // {
        //     case player.CharacterState.mage:
        //         imageMage.color = new Color(0,1,0);
        //         imageSlinger.color = new Color(1,1,1);
        //         imageWarrior.color = new Color(1,1,1);
        //         break;
        //     case player.CharacterState.slinger:
        //         imageMage.color = new Color(1,1,1);
        //         imageSlinger.color = new Color(0,1,0);
        //         imageWarrior.color = new Color(1,1,1);
        //         break;
        //     case player.CharacterState.warrior:
        //         imageMage.color = new Color(1,1,1);
        //         imageSlinger.color = new Color(0,1,0);
        //         imageWarrior.color = new Color(1,1,1);
        //         break;
        // }
    }

}

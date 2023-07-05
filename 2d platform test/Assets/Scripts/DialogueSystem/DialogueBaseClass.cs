using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBaseClass : MonoBehaviour
{
    protected IEnumerator WriteText(string input, TextMeshProUGUI textHolder, Color textcolor, TMP_FontAsset textfont, float delay)
    {
        textHolder.color = textcolor;
        textHolder.font = textfont;
        for (int i=0; i<input.Length; i++)
        {
            textHolder.text += input[i];
            yield return new WaitForSeconds(delay);
        }
    }
}

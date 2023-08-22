using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBaseClass : MonoBehaviour
{
    public bool finished { get; private set; }
    protected IEnumerator WriteText(string input, TextMeshProUGUI textHolder, Color textcolor, TMP_FontAsset textfont, float delay, float delayBetweenLines, string soundName)
    {
        textHolder.color = textcolor;
        textHolder.font = textfont;
        for (int i=0; i<input.Length; i++)
        {
            if (input[i] != '#')
            {
                textHolder.text += input[i];
                if (input[i] != ' ') AudioManager.Instance.PlaySFX(soundName);
                yield return new WaitForSeconds(delay);
            }
            else yield return new WaitForSeconds(delay * 3);
        }

        if (delayBetweenLines == 0) yield return new WaitUntil(() => Input.GetButtonDown("Jump"));
        else yield return new WaitForSeconds(delayBetweenLines);

        finished = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            if (input[i] != '#' && input[i] != '$')
            {
                textHolder.text += input[i];
                if (input[i] != ' ') AudioManager.Instance.PlaySFX(soundName);
                yield return new WaitForSeconds(delay);
            }
            else if (input[i] == '#') yield return new WaitForSeconds(delay * 3);
            else if (input[i] == '$') SceneManager.LoadScene("Menu");
        }

        if (delayBetweenLines == 0) yield return new WaitUntil(() => Input.GetButtonDown("Jump"));
        else yield return new WaitForSeconds(delayBetweenLines);

        finished = true;
    }
}

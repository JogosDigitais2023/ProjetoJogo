using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLine : DialogueBaseClass
{
    [SerializeField] private string input;
    [SerializeField] private Color color;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private float delay;
    private TextMeshProUGUI textHolder;

    private void Start()
    {
        textHolder = GetComponent<TextMeshProUGUI>();

        StartCoroutine(WriteText(input, textHolder, color, font, delay));
    }
}

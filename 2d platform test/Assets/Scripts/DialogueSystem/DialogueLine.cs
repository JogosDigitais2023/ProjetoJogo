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
    [SerializeField] private float delayBetweenLines;
    [SerializeField] private Sprite characterSprite;
    [SerializeField] private Image imageHolder;
    [SerializeField] private string sound;

    private TextMeshProUGUI textHolder;

    private void Awake()
    {
        textHolder = GetComponent<TextMeshProUGUI>();
        textHolder.text = "";
        imageHolder.sprite = characterSprite;
        imageHolder.preserveAspect = true;
    }

    private void Start()
    {
        StartCoroutine(WriteText(input, textHolder, color, font, delay, delayBetweenLines, sound));
    }
}
 
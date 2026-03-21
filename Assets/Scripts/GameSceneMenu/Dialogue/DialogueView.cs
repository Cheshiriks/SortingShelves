using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogueView
{
    public GameObject root;
    public RectTransform girlRect;
    public RectTransform bubbleRect;
    public TextMeshProUGUI dialogueText;
    
    [Header("Girl Enter Animation")]
    public DialogueEnterDirection enterDirection = DialogueEnterDirection.Left;
    public float hiddenX = -500f;
    public float hiddenY = -1300;
    
    [HideInInspector] public Vector2 girlShownPosition;
    [HideInInspector] public Vector3 bubbleShownScale;
}

public enum DialogueEnterDirection
{
    Left,
    Bottom
}
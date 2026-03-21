using UnityEngine;

public class LevelDialogueTrigger : MonoBehaviour
{
    public DialogueLang[] lines;

    public bool playOnEnable = true;
    public bool playOnlyOnce = false;

    private bool hasPlayed = false;

    private void OnEnable()
    {
        if (playOnEnable)
            TryPlay();
    }

    public void TryPlay()
    {
        if (playOnlyOnce && hasPlayed)
            return;

        if (lines == null || lines.Length == 0)
            return;

        hasPlayed = true;
        DialogueManager.Instance.StartDialogue(lines);
    }
}

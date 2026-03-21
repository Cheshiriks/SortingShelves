using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Views")]
    [SerializeField] private DialogueView portraitView;
    [SerializeField] private DialogueView landscapeView;
    [SerializeField] private bool squareIsLandscape = true;
    [SerializeField] private UIBlocker uiBlocker;

    [Header("Animation")]
    [SerializeField] private float girlMoveDuration = 0.45f;
    [SerializeField] private float bubbleShowDuration = 0.45f;

    private DialogueView currentView;

    private DialogueLang[] currentLines;
    private int currentIndex;
    private bool isOpen;
    private bool isAnimating;
    private bool isInitialized;

    private int lastScreenWidth;
    private int lastScreenHeight;

    public bool IsOpen => isOpen;
    public bool CanClick => isOpen && !isAnimating;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        EnsureInitialized();
        HideAllViews();
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            RefreshView(force: false);
        }
    }

    public void StartDialogue(DialogueLang[] lines)
    {
        EnsureInitialized();

        if (lines == null || lines.Length == 0)
            return;

        uiBlocker.SetBlocked(true);
        
        currentLines = lines;
        currentIndex = 0;

        StopAllCoroutines();
        StartCoroutine(OpenDialogueRoutine());
    }

    public void NextLine()
    {
        if (!isOpen || isAnimating)
            return;

        currentIndex++;

        if (currentIndex >= currentLines.Length)
        {
            CloseDialogue();
            return;
        }

        ShowCurrentLine();
    }

    public void OnDialogueClick()
    {
        NextLine();
    }

    private void EnsureInitialized()
    {
        if (isInitialized)
            return;

        Canvas.ForceUpdateCanvases();

        CacheViewDefaults(portraitView);
        CacheViewDefaults(landscapeView);

        RefreshView(force: true);
        isInitialized = true;
    }

    private IEnumerator OpenDialogueRoutine()
    {
        RefreshView(force: true);

        isOpen = true;
        isAnimating = true;

        currentView.root.SetActive(true);

        currentView.bubbleRect.localScale = Vector3.zero;

        Vector2 startPos = GetGirlHiddenPosition(currentView);
        currentView.girlRect.anchoredPosition = startPos;

        currentView.dialogueText.text = "";

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / girlMoveDuration;
            float eased = EaseOutCubic(Mathf.Clamp01(t));
            currentView.girlRect.anchoredPosition =
                Vector2.LerpUnclamped(startPos, currentView.girlShownPosition, eased);
            yield return null;
        }

        currentView.girlRect.anchoredPosition = currentView.girlShownPosition;

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / bubbleShowDuration;
            float eased = EaseOutBack(Mathf.Clamp01(t));
            currentView.bubbleRect.localScale =
                Vector3.LerpUnclamped(Vector3.zero, currentView.bubbleShownScale, eased);
            yield return null;
        }

        currentView.bubbleRect.localScale = currentView.bubbleShownScale;

        ShowCurrentLine();
        isAnimating = false;
    }

    private Vector2 GetGirlHiddenPosition(DialogueView view)
    {
        switch (view.enterDirection)
        {
            case DialogueEnterDirection.Bottom:
                return new Vector2(view.girlShownPosition.x, view.hiddenY);

            case DialogueEnterDirection.Left:
            default:
                return new Vector2(view.hiddenX, view.girlShownPosition.y);
        }
    }

    private void ShowCurrentLine()
    {
        if (currentLines == null || currentIndex < 0 || currentIndex >= currentLines.Length)
            return;

        currentView.dialogueText.text = GetLineText(currentLines[currentIndex]);
    }

    private string GetLineText(DialogueLang line)
    {
        bool isRussian = true;
        return isRussian ? line.LineRus : line.LineEng;
    }

    private void CloseDialogue()
    {
        uiBlocker.SetBlocked(false);
        
        isOpen = false;
        isAnimating = false;

        HideAllViews();

        currentLines = null;
        currentIndex = 0;
    }

    private void RefreshView(bool force)
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        DialogueView targetView = ShouldUseLandscape()
            ? landscapeView
            : portraitView;

        if (!force && currentView == targetView)
            return;

        currentView = targetView;

        HideAllViews();

        if (isOpen)
        {
            currentView.root.SetActive(true);
            currentView.girlRect.anchoredPosition = currentView.girlShownPosition;
            currentView.bubbleRect.localScale = currentView.bubbleShownScale;
            ShowCurrentLine();
        }
    }

    private bool ShouldUseLandscape()
    {
        if (squareIsLandscape)
            return Screen.width >= Screen.height;

        return Screen.width > Screen.height;
    }

    private void CacheViewDefaults(DialogueView view)
    {
        if (view == null || view.root == null)
            return;

        view.girlShownPosition = view.girlRect.anchoredPosition;
        view.bubbleShownScale = view.bubbleRect.localScale;
    }

    private void HideAllViews()
    {
        if (portraitView != null && portraitView.root != null)
            portraitView.root.SetActive(false);

        if (landscapeView != null && landscapeView.root != null)
            landscapeView.root.SetActive(false);
    }

    private float EaseOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}

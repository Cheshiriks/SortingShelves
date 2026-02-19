using UnityEngine;

public class UIBlocker : MonoBehaviour
{
    [SerializeField] private DragController dragController;

    public void SetBlocked(bool blocked)
    {
        if (dragController) dragController.enabled = !blocked;
    }
}

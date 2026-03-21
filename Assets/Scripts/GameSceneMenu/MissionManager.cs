using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [SerializeField] private GameObject[] missions;

    private void Start()
    {
        int level = SaveGame.Instance.GameLevel;
        ShowMission(level);
    }

    public void ShowMission(int level)
    {
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i] != null)
                missions[i].SetActive(false);
        }

        int index = level - 1;

        if (index < 0 || index >= missions.Length)
        {
            Debug.LogError($"Миссия для уровня {level} не найдена.");
            missions[0].SetActive(true);
            return;
        }

        missions[index].SetActive(true);
    }
}

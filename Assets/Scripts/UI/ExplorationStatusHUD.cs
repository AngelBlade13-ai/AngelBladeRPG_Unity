using TMPro;
using UnityEngine;

public class ExplorationStatusHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (statusText == null)
        {
            return;
        }

        statusText.text = FormatPlayerStatus(GameSessionStore.Current.Player);
    }

    public static string FormatPlayerStatus(PlayerData player)
    {
        if (player == null)
        {
            return "";
        }

        return
            $"{player.Name}  Lv {player.Level}\n" +
            $"HP {player.CurrentHp}/{player.MaxHp}   " +
            $"XP {player.XP}/{player.XPToNextLevel}   " +
            $"Gold {player.Gold}";
    }
}

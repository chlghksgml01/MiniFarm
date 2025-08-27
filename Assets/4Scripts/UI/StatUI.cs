using UnityEngine;
using UnityEngine.UI;

public enum StatType { Health, Stamina }

public class StatUI : MonoBehaviour
{
    [SerializeField] public Image healthBar;
    [SerializeField] public Image staminaBar;

    private Player player;

    private void OnEnable()
    {
        player = InGameManager.Instance.player;
        player.OnStatChanged += ChangeStatUI;
    }

    private void OnDisable()
    {
        player.OnStatChanged -= ChangeStatUI;
    }

    public void ChangeStatUI(StatType statType)
    {
        if (statType == StatType.Health)
            SetGague(healthBar, player.playerSaveData.hp, player.maxHp);
        else
            SetGague(staminaBar, player.playerSaveData.stamina, player.maxStamina);
    }

    public void SetGague(Image gauge, int value, int maxValue)
    {
        if (value <= 0)
        {
            gauge.fillAmount = 0f;
            return;
        }

        float percent = (float)value / maxValue;

        for (float i = 8f; i >= 1f; i--)
        {
            if (percent >= 0.125f * i)
            {
                gauge.fillAmount = i / 8f;
                break;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image cooldownSprite;

    public void UpdateUI(float currentTime, float cooldown, bool isOnCooldown)
    {
        if(!isOnCooldown)
            cooldownSprite.fillAmount = 0f;
        else
            cooldownSprite.fillAmount = 1 - currentTime / cooldown;
    }
}

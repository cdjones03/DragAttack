using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public int maxHP = 10;
    public int currentHP;
    public Image[] heartIcons;

    private void Start()
    {
        currentHP = maxHP;
        UpdateHPBar();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        UpdateHPBar();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (i < currentHP)
            {
                heartIcons[i].enabled = true;
            }
            else
            {
                heartIcons[i].enabled = false;
            }
        }
    }
}

using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private BattleField battleField;
    private bool hasBeenActivated = false;

    void Start()
    {
        // Get the parent's BattleField component
        battleField = GetComponentInParent<BattleField>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && battleField != null && !battleField.isBattleActive && !hasBeenActivated)
        {
            battleField.StartBattle();
            hasBeenActivated = true;
        }
    }
} 
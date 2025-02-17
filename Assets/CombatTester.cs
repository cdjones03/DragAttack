using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTester : MonoBehaviour
{
    [SerializeField] private bool canAttack = true;
    
    [SerializeField] private Collider2D inLineCollider;
    
    [SerializeField] private LayerMask enemyLayer;
    private ContactFilter2D contactFilter2D;
    public List<Collider2D> cols = new List<Collider2D>();

    private bool attackInput;
    
    private void Awake()
    {
        contactFilter2D.SetLayerMask(enemyLayer);
    }


    // Update is called once per frame
    void Update()
    {
        attackInput = UserInput.instance.attackInput;
        if (attackInput)
        {
            inLineCollider.OverlapCollider(contactFilter2D, cols);
            if (cols.Count > 0)
            {
                foreach (var col in cols)
                {
                    print(col.transform.name);
                    if (col.TryGetComponent(out SpriteRenderer sr))
                    {
                        sr.color = Color.red;
                    }
                }
            }
        }
    }

}

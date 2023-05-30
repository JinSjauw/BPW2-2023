using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class EnemyDetect : MonoBehaviour
{
    [SerializeField] private float detectRadius;
    private LayerMask targetLayers;

    private Unit unit;
    // Start is called before the first frame update
    void Start()
    {
        LayerMask layerPlayer = LayerMask.GetMask("Player");
        LayerMask layerEnemies = LayerMask.GetMask("Enemy");

        unit = GetComponent<Unit>();
        
        //targetLayers = (1 << layerPlayer) | (1 << layerEnemies);
        targetLayers = layerPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if player comes close

        if (unit.GetState() != Unit.UnitState.IDLE)
        {
            return;
        }
        
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, targetLayers);
        if(hits.Length > 0)
        {
            //Debug.Log(hits.Length);
            foreach (var col in hits)
            {
                Unit hitUnit = col.transform.root.GetComponent<Unit>();
                //Debug.Log(hitUnit.name);
                if (hitUnit == null)
                {
                    continue;
                }
                
                if (!hitUnit.IsEnemy())
                {
                    //Debug.Log("Detected Player");
                    hitUnit.SetState(Unit.UnitState.COMBAT);
                    unit.Alert();
                }
            }
        }
        //Alert other Units in the vicinity
        //Apply red outline to show enemy is InCombat
    }
}

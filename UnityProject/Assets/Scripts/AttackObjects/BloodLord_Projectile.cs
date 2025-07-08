using Unity.VisualScripting;
using UnityEngine;

public class BloodLord_Projectile : MonoBehaviour
{
    AttackObject ao;
    PlayerControll ps;
    private bool isCured = false;
    private void Start()
    {
        ao = GetComponent<AttackObject>();
        ps = GameObject.Find("Player").GetComponent<PlayerControll>();
    }
    private void FixedUpdate()
    {
        if (ao.isHit && !isCured)
        {
            isCured = true;
            ps.ModifyHealth(2f);
        }
    }
}

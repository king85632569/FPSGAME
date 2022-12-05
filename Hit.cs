using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{
    public GameObject Player;
    //public GameObject Enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Bubble_Addition()
    {
        Player.GetComponent<IDamageable>().Bubble_Addition();
    }
    public void TakeDamage_Monster(float damage)
    {
        Player.GetComponent<IDamageable>().TakeDamage_Monster(damage);
    }

    public void Hit_Trigger(float damage,bool Team, bool BloodAddition, PlayerManager Enemy, bool DizzyState)
    {
        if ((Team != Player.GetComponent<PlayerController>().Team)&& (BloodAddition == false))
        {
            Player.GetComponent<IDamageable>().TakeDamage(damage, Enemy);
            Player.GetComponent<IDamageable>().Hurt_Show(damage,false);
            if(DizzyState)
            {
                Player.GetComponent<IDamageable>().SetDizzy(true);
            }
        }
        if ((Team == Player.GetComponent<PlayerController>().Team)&&(BloodAddition==true))
        {
            Player.GetComponent<IDamageable>().Hurt_Show(damage, true);
            Player.GetComponent<IDamageable>().BloodAddition(damage);
        }
    }
}

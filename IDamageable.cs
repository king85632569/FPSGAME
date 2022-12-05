using UnityEngine;
public interface IDamageable
{
	void TakeDamage(float damage, PlayerManager Murderer);
	void Hurt_Show(float Hurt, bool BloodAddition);
	void BloodAddition(float damage);
	void SetDizzy(bool DizzyState);
	void SetFall_Damage(bool Fall_Damage_State);
	void Bubble_Addition();
	void TakeDamage_Monster(float damage);
}
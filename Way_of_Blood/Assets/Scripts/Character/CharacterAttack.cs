using UnityEngine;
using UnityEngine.Events;
using WayOfBlood.Character;

public class CharacterAttack : MonoBehaviour
{
    public event UnityAction OnAttack;
    public event UnityAction<CharacterHealth> OnDamage;

    [Header("Attack parameters")]
    public int AttackDamage;                                // Урон
    public float AttackCooldown = 0.1f;                     // Кулдаун

    private CharacterMovement characterMovement;
    private float lastAttackTime;
    private bool attackActivity = false;                    // Активность атаки (1 атака - 1 враг)

    protected virtual void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    public void Attack()
    {
        if (Time.time > lastAttackTime + AttackCooldown)
        {
            lastAttackTime = Time.time;
            attackActivity = true;
            OnAttack?.Invoke();
        }
    }

    // Вызывается из AttackHitbox при попадании
    public void NotifyHit(CharacterHealth enemyHealth)
    {
        enemyHealth.TakeDamage(AttackDamage);
        OnDamage?.Invoke(enemyHealth);
        attackActivity = false;
    }

    protected virtual void OnDestroy()
    {
        OnAttack = null;
        OnDamage = null;
    }
}
using UnityEngine;
using WayOfBlood.Character;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private CharacterAttack characterAttack;
    [SerializeField] private LayerMask enemyLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что объект находится на одном из слоев enemyLayer
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent<CharacterHealth>(out var enemyHealth))
            {
                characterAttack.NotifyHit(enemyHealth); // Уведомляем основной скрипт
            }
        }
    }
}
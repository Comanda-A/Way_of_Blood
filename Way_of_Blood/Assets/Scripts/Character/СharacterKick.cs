using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace WayOfBlood.Character
{
    public class СharacterKick : MonoBehaviour
    {
        public event UnityAction OnKick;        // Событие удара ногой 
        public float KickingRadius = 1f;        // Дистанция для удара ногой
        public float AngleKick = 20;            // Угол для удара ногой
        public float KickCooldown = 0.1f;       // Задержка между ударами
        public float SpeedKickTarget;           // Скорость цели
        public float DurationKickTarget;        // Продолжительность цели

        public LayerMask KickMask;

        protected float lastKickTime = 0f;      // Время последнего удара ногой
        protected new Transform transform;
        protected CharacterMovement characterMovement;

        protected virtual void Start()
        {
            transform = GetComponent<Transform>();
            characterMovement = GetComponent<CharacterMovement>();
        }

        public void Kick()
        {
            if (Time.time > lastKickTime + KickCooldown)
            {
                ProcessKick();
                OnKick?.Invoke();
                lastKickTime = Time.time;
            }
        }

        protected virtual void ProcessKick()
        {
            // Получаем направление взгляда игрока
            Vector2 kickDirection = characterMovement.ViewDirection;

            // Находим всех врагов в радиусе атаки
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, KickingRadius, KickMask);

            // Проходим по всем найденным врагам
            foreach (var enemy in hitEnemies)
            {
                // Вычисляем направление от игрока к врагу
                Vector2 directionToEnemy = (enemy.transform.position - transform.position).normalized;

                // Проверяем, находится ли враг в пределах угла атаки
                if (Vector2.Angle(kickDirection, directionToEnemy) < AngleKick / 2)
                {
                    Transform target = enemy.transform;

                    if (target.TryGetComponent<CharacterMovement>(out var component))
                    {
                        component.SetForcedMovement(
                            target.position - transform.position,
                            component.ViewDirection,
                            DurationKickTarget,
                            SpeedKickTarget,
                            component.CurrentAcceleration,
                            true
                        );

                        break;
                    }
                }
            }
        }

        protected virtual void OnDestroy()
        {
            OnKick = null;
        }
    }
}

using UnityEngine;

namespace WayOfBlood.Character.Enemy
{
    public class SwordEnemy : EnemyController
    {
        [Header("Settings")]
        public float AttackRange = 2f;              // Дистанция для атаки мечом

        private CharacterMovement _playerMovement;
        private EnemyAttack _enemyAttack;

        protected override void Start()
        {
            base.Start();
            _playerMovement = _player.GetComponent<CharacterMovement>();
            _enemyAttack = GetComponent<EnemyAttack>();
        }

        private void Update()
        {
            if (_player == null || IsDead)
            {
                _enemyMovement.StopMovement();
                return;
            }

            // Вычисляем направление к ближайшему игроку
            Vector2 directionToPlayer = ((Vector2)(_player.position - transform.position)).normalized;

            // Движение к ближайшему игроку
            if (Vector2.Distance(transform.position, _player.position) > AttackRange)
            {
                _enemyMovement.MoveToPosition(_player.position);
            }
            else
            {
                _enemyMovement.StopMovement(); // Останавливаемся для атаки
            }

            // Атака мечом, если близко к игроку
            if (Vector2.Distance(transform.position, _player.position) <= AttackRange)
            {
                _enemyAttack.Attack();
            }

            // Проверка на необходимость уклонения от пуль
            CheckForBulletEvasion();

            // Проверка на необходимость переката, если игрок слишком близко
            if (Vector2.Distance(transform.position, _player.position) <= AttackRange * 0.8f)
            {
                //PerformRoll(GetEvadeDirection(_playerMovement.MoveDirection));
            }
        }
    }
}
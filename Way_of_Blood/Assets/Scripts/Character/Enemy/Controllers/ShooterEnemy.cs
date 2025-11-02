using UnityEngine;

namespace WayOfBlood.Character.Enemy
{
    public class ShooterEnemy : EnemyController
    {
        [Header("Settings")]
        public float ShootRange = 5f;                   // Дистанция для стрельбы
        public float SafeDistance = 3f;                 // Минимальная дистанция до игрока
        public float CircleSpeed = 2f;                  // Скорость движения вокруг игрока

        protected CharacterMovement _playerMovement;
        protected EnemyShot _enemyShot;

        protected override void Start()
        {
            base.Start();
            _enemyShot = GetComponent<EnemyShot>();
            _playerMovement = _player.GetComponent<CharacterMovement>();
        }

        private void Update()
        {
            if (_player == null || IsDead)
            {
                _enemyMovement.StopMovement();
                return;
            }

            float distanceToPlayer = Vector2.Distance(_transform.position, _player.position);
            Vector2 directionToPlayer = ((Vector2)(_player.position - _transform.position)).normalized;

            if (distanceToPlayer > ShootRange)
            {
                _enemyMovement.MoveToPosition(_player.position);
            }
            else if (distanceToPlayer < SafeDistance)
            {
                Vector2 escapeDirection = ((Vector2)(_transform.position - _player.position)).normalized;
                _enemyMovement.MoveToPosition((Vector2)_transform.position + escapeDirection * 2);
            }
            else
            {
                MoveAroundPlayer();
                _enemyShot.Shot(_transform.position, directionToPlayer);
            }

            CheckForBulletEvasion();

            if (distanceToPlayer < SafeDistance * 0.8f)
            {
                //PerformRoll(GetEvadeDirection(_playerMovement.MoveDirection));
            }
        }

        private void MoveAroundPlayer()
        {
            Vector2 perpendicularDirection = new Vector2(-(_player.position - _transform.position).y, (_player.position - _transform.position).x).normalized;
            _enemyMovement.MoveToPosition((Vector2)_transform.position + perpendicularDirection * CircleSpeed * Time.deltaTime);
        }
    }
}

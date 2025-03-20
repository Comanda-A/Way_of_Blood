using UnityEngine;
using UnityEngine.InputSystem;
using WayOfBlood.ControlInputSystem;
using System.Collections;

namespace WayOfBlood.Character.Player
{
    public class PlayerShot : CharacterShot
    {
        [Header("Shooting settings")]
        [GoogleSheetsVariable("config", "BulletRadiusAtShot", "Value")]
        public float BulletRadiusAtShot = 0.8f;     // Радиус стрельбы

        [Header("Auto aiming")]
        public float AimAssistRadius = 15f;         // Радиус помощи прицеливания
        public float AimAssistAngle = 45f;          // Угол помощи прицеливания
        public float AimLockStrength = 10f;         // Сила доводки прицела
        public LayerMask EnemyLayerMask;            // Слой с врагами

        private Transform _transform;
        private Camera _mainCamera;
        private PlayerMovement _playerMovement;
        private PlayerBlood _playerBlood;

        private ControlInput _controlInput;
        private InputAction _shotAction;
        private InputAction _changeShootingModeAction;

        private bool _aimMode;  // Режим прицеливания
        private Coroutine _autoAimCoroutine;

        void Start()
        {
            _mainCamera = Camera.main;
            _transform = GetComponent<Transform>();
            _playerMovement = GetComponent<PlayerMovement>();
            _playerBlood = GetComponent<PlayerBlood>();
            _controlInput = GetComponent<ControlInput>();

            _shotAction = InputSystem.actions.FindAction("Shot");
            _shotAction.performed += ShotHandler;

            _changeShootingModeAction = InputSystem.actions.FindAction("ChangeShootingMode");
            _changeShootingModeAction.performed += ChangeShootingModeHandler;

            _aimMode = false;
        }

        // Метод для получения цели для автоприцеливания
        private Transform GetAutoAimTarget(Vector2 viewDirection)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(_transform.position, AimAssistRadius, EnemyLayerMask);

            Transform bestTarget = null;
            float closestAngle = AimAssistAngle / 2;
            float closestDistance = Mathf.Infinity;

            foreach (var enemy in enemies)
            {
                Vector2 directionToEnemy = (enemy.transform.position - _transform.position).normalized;
                float angle = Vector2.Angle(viewDirection, directionToEnemy);

                if (angle < closestAngle)
                {
                    float distance = Vector2.Distance(_transform.position, enemy.transform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestTarget = enemy.transform;
                    }
                }
            }

            return bestTarget;
        }

        private void ShotHandler(InputAction.CallbackContext context)
        {
            if (Time.time < lastShotTime + ShotCooldown || _playerBlood.Blood == 0)
                return;

            Vector2 direction = Vector2.zero;

            switch (_controlInput.CurrentInputType)
            {
                case ControlInput.InputType.Keyboard:
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                    direction = ((Vector2)(mousePosition - _transform.position)).normalized;
                    break;
                case ControlInput.InputType.Gamepad:
                case ControlInput.InputType.Touch:
                    direction = _playerMovement.ViewDirection;
                    break;
            }

            _playerBlood.TakeBlood(1);
            Shot((Vector2)_transform.position + direction * BulletRadiusAtShot, direction);
        }

        private void ChangeShootingModeHandler(InputAction.CallbackContext context)
        {
            _aimMode = !_aimMode;

            if (_aimMode)
            {
                _playerMovement.SetMoveDirectionForChanges(false, Vector2.zero);
                _playerMovement.ViewDirectionSetting = PlayerMovement.ViewDirectionMode.Free;
                StartAutoAim();
            }
            else
            {
                _playerMovement.SetMoveDirectionForChanges(true, Vector2.zero);
                _playerMovement.ViewDirectionSetting = PlayerMovement.ViewDirectionMode.FourDirections;
                StopAutoAim();
            }
        }

        // Корутин для автоприцеливания
        private void StartAutoAim()
        {
            if (_autoAimCoroutine != null)
            {
                StopCoroutine(_autoAimCoroutine);
            }

            _autoAimCoroutine = StartCoroutine(AutoAimRoutine());
        }

        private void StopAutoAim()
        {
            if (_autoAimCoroutine != null)
            {
                StopCoroutine(_autoAimCoroutine);
                _autoAimCoroutine = null;
            }
        }

        private IEnumerator AutoAimRoutine()
        {
            while (_aimMode)
            {
                Transform autoAimTarget = GetAutoAimTarget(_playerMovement.GetViewDirectionInput());

                if (autoAimTarget != null)
                {
                    _playerMovement.ViewDirectionSetting = PlayerMovement.ViewDirectionMode.AutoAim;
                    _playerMovement.SetAutoAimTarget(autoAimTarget, AimLockStrength);
                }
                else
                {
                    _playerMovement.ViewDirectionSetting = PlayerMovement.ViewDirectionMode.Free;
                }
                    
                yield return null;
            }

            yield break;
        }

        private void OnDestroy()
        {
            _shotAction.performed -= ShotHandler;
        }
    }
}

using UnityEngine;

namespace WayOfBlood.Character
{
    public class CharacterDebug : MonoBehaviour
    {
        /// <summary>
        /// Отладка сущности.
        /// </summary>
        public bool DebugEnabled;

        [Header("CharacterMovement")]
        public bool DebugMoveDirection;
        public bool DebugViewDirection;
        private CharacterMovement _characterMovement;

        private void Start()
        {
            TryGetComponent(out _characterMovement);
        }

        private void Update()
        {
            if (!DebugEnabled)
                return;

            DebugCharacterMovement();
        }

        private void DebugCharacterMovement()
        {
            if (_characterMovement == null)
                return;

            Vector2 position = transform.position;

            if (DebugMoveDirection)
            {
                Debug.DrawLine(position, position + _characterMovement.MoveDirection * 2f, Color.green);
            }

            if (DebugViewDirection)
            {
                Debug.DrawLine(position, position + _characterMovement.ViewDirection * 4f, Color.red);
            }
        }
    }
}

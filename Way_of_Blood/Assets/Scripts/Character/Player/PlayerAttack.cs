using UnityEngine;
using UnityEngine.InputSystem;

namespace WayOfBlood.Character.Player
{
    public class PlayerAttack : CharacterAttack
    {
        [Header("Bloodlust bonus on the kill")]
        public int BloodBonusForKill = 1;

        private PlayerBlood playerBloodlust;
        private InputAction attackAction;

        protected override void Start()
        {
            base.Start();
            playerBloodlust = GetComponent<PlayerBlood>();
            attackAction = InputSystem.actions.FindAction("Attack");
            attackAction.performed += AttackHandler;
            OnDamage += DamageHandler;
        }

        private void AttackHandler(InputAction.CallbackContext context)
        {
            Attack();
        }

        private void DamageHandler(CharacterHealth characterHealth)
        {
            playerBloodlust.AddBlood(BloodBonusForKill);
            Camera.main.GetComponent<CameraController>().ShakeCamera();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            attackAction.performed -= AttackHandler;
            OnDamage -= DamageHandler;
        }
    }
}
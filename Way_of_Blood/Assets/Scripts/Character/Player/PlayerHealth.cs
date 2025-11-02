using System.Collections;
using UnityEngine;

namespace WayOfBlood.Character.Player
{
    public class PlayerHealth : CharacterHealth
    {
        [Header("Shields parameters")]
        public int ShieldsCount;   // Количество щитов
        public int ShieldCost;     // Стоимость щита в единицах крови

        private CharacterBlood  _characterBlood;
        private Coroutine       _regenerationCoroutine;

        protected override void Start()
        {
            base.Start();

            _characterBlood = GetComponent<CharacterBlood>();

            OnHealthChange += OnHealthChangeHandler;
        }

        public override void TakeDamage(int damage)
        {
            if (!_characterController.IsDead)
            {
                if (_characterBlood.Blood <= 0)
                    base.TakeDamage(damage);
                else 
                    _characterBlood.TakeBlood(damage);
            }  
        }


        private void OnHealthChangeHandler(int newHealth)
        {
            if (Health == 0)
            {
                GetComponent<PlayerController>().Die();
                return;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            OnHealthChange -= OnHealthChangeHandler;
        }
    }
}
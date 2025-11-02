using UnityEngine;
using UnityEngine.Events;


namespace WayOfBlood.Character
{
    public class CharacterBlood : MonoBehaviour
    {
        public event UnityAction<int> OnMaxBloodChange;    // <int> is new value
        public event UnityAction<int> OnBloodChange;       // <int> is new value

        [SerializeField] private int _maxBlood;

        public int MaxBlood
        {
            private set { _maxBlood = value; OnMaxBloodChange?.Invoke(_maxBlood); }
            get { return _maxBlood; }
        }

        [SerializeField] private int _blood;

        public int Blood
        {
            private set { _blood = value; OnBloodChange?.Invoke(_blood); }
            get { return _blood; }
        }

        public void AddBlood(int value) // добавить жажду крови
        {
            Blood = Blood + value <= MaxBlood ? Blood + value : MaxBlood;
        }

        public void TakeBlood(int value) // взять жажду крови (уменьшить)
        {
            Blood = Blood >= value ? Blood - value : 0;
        }
    }
}

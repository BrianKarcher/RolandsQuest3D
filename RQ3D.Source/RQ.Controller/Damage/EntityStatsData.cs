using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Damage
{
    [Serializable]
    public class EntityStatsData
    {
        [SerializeField]
        private float _maxHP;
        public float MaxHP { get { return _maxHP; } set { _maxHP = value; } }

        [SerializeField]
        private float _currentHP;
        public float CurrentHP { get { return _currentHP; } set { _currentHP = value; } }

        [SerializeField]
        private float _maxSP;
        public float MaxSP { get { return _maxSP; } set { _maxSP = value; } }

        [SerializeField]
        private float _currentSP;
        public float CurrentSP { get { return _currentSP; } set { _currentSP = value; } }

        [SerializeField]
        private float _maxStamina;
        public float MaxStamina { get => _maxStamina; set => _maxStamina = value;  }

        [SerializeField]
        private float _currentStamina;
        public float CurrentStamina { get => _currentStamina; set => _currentStamina = value; }

        [SerializeField]
        private List<ActionStaminaTuple> _actionStaminas;
        public List<ActionStaminaTuple> ActionStaminas { get => _actionStaminas; set => _actionStaminas = value; }

        [SerializeField]
        private float _staminaRefreshAmount;
        public float StaminaRefreshAmount { get => _staminaRefreshAmount; set => _staminaRefreshAmount = value; }

        [SerializeField]
        private float _staminaRefreshDelay;
        public float StaminaRefreshDelay { get => _staminaRefreshDelay; set => _staminaRefreshDelay = value; }

        [SerializeField]
        private float _physicalAttackPower;
        public float PhysicalAttackPower { get { return _physicalAttackPower; } set { _physicalAttackPower = value; } }

        [SerializeField]
        private float _magicalAttackPower;
        public float MagicalAttackPower { get { return _magicalAttackPower; } set { _magicalAttackPower = value; } }

        [SerializeField]
        private float _physicalDefense;
        public float PhysicalDefense { get { return _physicalDefense; } set { _physicalDefense = value; } }

        [SerializeField]
        private float _magicalDefense;
        public float MagicalDefense { get { return _magicalDefense; } set { _magicalDefense = value; } }

        [SerializeField]
        private int _experience;
        public int Experience { get { return _experience; } set { _experience = value; } }

        [SerializeField]
        private int _level;
        public int Level { get { return _level; } set { _level = value; } }

        public bool IsHiding;

        // TODO - Find a way to easily serialize this.
        //[JsonIgnore]
        //[SerializeField]
        //private DropItem[] _dropItems;
        //[JsonIgnore]
        //public DropItem[] DropItems { get { return _dropItems; } set { _dropItems = value; } }

        //[SerializeField]
        //private int _creationOrbCount;
        //public int CreationOrbCount { get { return _creationOrbCount; } set { _creationOrbCount = value; } }

        //[SerializeField]
        //private int _justiceOrbCount;
        //public int JusticeOrbCount { get { return _justiceOrbCount; } set { _justiceOrbCount = value; } }

        //[SerializeField]
        //private int _temperanceOrbCount;
        //public int TemperanceOrbCount { get { return _temperanceOrbCount; } set { _temperanceOrbCount = value; } }

        //[SerializeField]
        //private int _spiritOrbCount;
        //public int SpiritOrbCount { get { return _spiritOrbCount; } set { _spiritOrbCount = value; } }

        //public Dictionary<string, string> Variables { get; set; }

        public EntityStatsData Clone()
        {
            return new EntityStatsData()
            {
                _maxHP = this._maxHP,
                _currentHP = this._currentHP,
                _maxSP = this._maxSP,
                _currentSP = this._currentSP,
                //_dropItems = this._dropItems,
                _experience = this._experience,
                _level = this._level,
                _magicalAttackPower = this._magicalAttackPower,
                _magicalDefense = this._magicalDefense,
                _physicalAttackPower = this._physicalAttackPower,
                _physicalDefense = this._physicalDefense
            };
        }
    }
}

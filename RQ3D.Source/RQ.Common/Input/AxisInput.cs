using System;
using UnityEngine;

namespace RQ.Base.Input
{
    [Serializable]
    public class AxisInput
    {
        [SerializeField]
        private string _axisName;
        [SerializeField]
        private float _deadzone = 0.3f;
        private AxisExtended _currentAxisExtended;
        //private bool _axisExtendedPositive = false;
        //private bool _axisExtendedNegative = false;
        private Rewired.Player _player;

        public void Init()
        {
            _player = Rewired.ReInput.players.GetPlayer(0);
        }

        public enum AxisExtended
        {
            Negative = 0,
            None = 1,
            Positive = 2
        }

        public AxisExtended GetAxisDown()
        {
            AxisExtended rtn = AxisExtended.None;
            var currentAxis = _player.GetAxis(_axisName);
            if (currentAxis < -_deadzone && _currentAxisExtended != AxisExtended.Negative)
            {
                rtn = AxisExtended.Negative;
                _currentAxisExtended = AxisExtended.Negative;
                //uIToggleGroup.Toggle(false);
                //newItem = ToggleDirection == ToggleDirection.Right ? CurrentItem - 1 : CurrentItem + 1;
                //SetCurrentItem(newItem);
                //_axisExtendedLeft = true;
                //_axisExtendedRight = false;
            }
            if (currentAxis > -_deadzone && currentAxis < _deadzone)
            {
                _currentAxisExtended = AxisExtended.None;
                //_axisExtendedLeft = false;
                //_axisExtendedRight = false;
            }
            if (currentAxis > _deadzone && _currentAxisExtended != AxisExtended.Positive)
            {
                rtn = AxisExtended.Positive;
                _currentAxisExtended = AxisExtended.Positive;
                //uIToggleGroup.Toggle(true);
                //newItem = ToggleDirection == ToggleDirection.Right ? CurrentItem + 1 : CurrentItem - 1;
                //SetCurrentItem(newItem);
                //_axisExtendedLeft = false;
                //_axisExtendedRight = true;
            }
            return rtn;
        }
    }
}

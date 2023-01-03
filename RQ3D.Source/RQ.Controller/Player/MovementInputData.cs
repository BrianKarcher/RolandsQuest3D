using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Controller.Player
{
    public struct MovementInputData
    {
        public Vector2 DirectionalInput_Raw { get; set; }
        /// <summary>
        /// the world-relative desired move direction, calculated from the camForward and user input.
        /// </summary>
        public Vector3 DirectionalInput_CameraRelative { get; set; }
        public bool JumpPressed { get; set; }
        public bool ParryPressed { get; set; }
        public bool SprintDown { get; set; }
        public bool SprintUp { get; set; }
        public bool TurnCrouchOn { get; set; }
        public bool TurnCrouchOff { get; set; }

        public void Clear()
        {
            DirectionalInput_CameraRelative = Vector3.zero;
            JumpPressed = false;
            SprintDown = false;
            SprintUp = false;
            TurnCrouchOn = false;
            TurnCrouchOff = false;
        }
    }
}

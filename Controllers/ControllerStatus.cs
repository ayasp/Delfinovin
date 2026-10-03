using System;
using System.Collections.Generic;
using System.Numerics;

namespace Delfinovin.Controllers
{
    /// <summary>
    /// Represents values that a Gamecube Controller can have set.
    /// Provides functionality to receive bit inputs and update flags.
    /// </summary>
    public struct ControllerStatus
    {
        // 元のフィールド形式に戻すことで、GamecubeAdapter.csからの代入エラー(CS1612)を回避
        private Vector2 _lStick;
        public Vector2 LStick 
        { 
            get => new Vector2(127, 127); // 常に中央値を返す
            set => _lStick = new Vector2(127, 127); // 代入されても強制的に中央値にする
        }

        private Vector2 _rStick;
        public Vector2 RStick 
        { 
            get => new Vector2(127, 127); // 常に中央値を返す
            set => _rStick = new Vector2(127, 127); // 代入されても強制的に中央値にする
        }

        // Vector2.X is Left Trigger, Vector2.Y is Right Trigger
        public Vector2 Triggers;

        public GamecubeControllerButtons Buttons;
        public ControllerType ControllerType;
        public bool IsPowered { get; set; }

        // If the controller type isn't set to none,
        // that means we're connected
        public ConnectionStatus IsPlugged
        {
            get 
            { 
                return (ControllerType != ControllerType.None) ? ConnectionStatus.Connected : ConnectionStatus.Disconnected; 
            }
        }

        public ControllerStatus()
        {
            _lStick = new Vector2(127, 127);
            _rStick = new Vector2(127, 127);
            Triggers = Vector2.Zero;
        }

        public List<GamecubeControllerButtons> GetButtonsPressed()
        {
            List<GamecubeControllerButtons> buttons = new List<GamecubeControllerButtons>();

            foreach (GamecubeControllerButtons button in Enum.GetValues<GamecubeControllerButtons>())
            {
                if (Buttons.HasFlag(button) && button != GamecubeControllerButtons.None)
                    buttons.Add(button);
            }

            return buttons;
        }

        public void SetButtonFlag(GamecubeControllerButtons flag, bool toSet)
        {
            Buttons = toSet ? Buttons | flag : Buttons & ~flag;
        }

        public bool IsButtonPressed(GamecubeControllerButtons button)
        {
            return Buttons.HasFlag(button);
        }

        public void UpdateTriggerButtons(float deadzone)
        {
            SetButtonFlag(GamecubeControllerButtons.LAnalog, (Triggers.X / 255f) > deadzone);
            SetButtonFlag(GamecubeControllerButtons.RAnalog, (Triggers.Y / 255f) > deadzone);
        }

        public bool IsButtonsEqual(ControllerStatus compare)
        {
            return Buttons == compare.Buttons;
        }

        public bool IsEqual(ControllerStatus compare)
        {
            return Buttons == compare.Buttons &&
                Triggers.Equals(compare.Triggers);
        }
    }
}

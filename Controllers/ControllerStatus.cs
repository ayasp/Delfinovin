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
        // タルコンガ等でスティックがない場合に暴走（左下に倒れっぱなし）するのを防ぐため、
        // 常に中央値（127, 127）を返すように固定します。
        public Vector2 LStick 
        { 
            get => new Vector2(127, 127); 
            set { } 
        }
        
        public Vector2 RStick 
        { 
            get => new Vector2(127, 127); 
            set { } 
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
            // 初期化時も念のためトリガーなどをゼロクリア
            Triggers = Vector2.Zero;
        }

        public List<GamecubeControllerButtons> GetButtonsPressed()
        {
            // Create a new list of buttons
            List<GamecubeControllerButtons> buttons = new List<GamecubeControllerButtons>();

            // If the bitfield contains an enum value, add it to the list of buttons pressed
            foreach (GamecubeControllerButtons button in Enum.GetValues<GamecubeControllerButtons>())
            {
                if (Buttons.HasFlag(button) && button != GamecubeControllerButtons.None)
                    buttons.Add(button);
            }

            // Return our button list
            return buttons;
        }

        public void SetButtonFlag(GamecubeControllerButtons flag, bool toSet)
        {
            // Bitwise OR the value if true. If not true, bitwise AND NOT the flag
            // so that the flag is removed from the bitfield.
            Buttons = toSet ? Buttons | flag : Buttons & ~flag;
        }

        public bool IsButtonPressed(GamecubeControllerButtons button)
        {
            // Check if the button is part of the Buttons bitwise field.
            return Buttons.HasFlag(button);
        }

        public void UpdateTriggerButtons(float deadzone)
        {
            // Set the LAnalog/RAnalog button values based on if they're in the deadzone value
            SetButtonFlag(GamecubeControllerButtons.LAnalog, (Triggers.X / 255f) > deadzone);
            SetButtonFlag(GamecubeControllerButtons.RAnalog, (Triggers.Y / 255f) > deadzone);
        }

        public bool IsButtonsEqual(ControllerStatus compare)
        {
            return Buttons == compare.Buttons;
        }

        public bool IsEqual(ControllerStatus compare)
        {
            // Compare if the buttons/stick values are the same
            // (スティックは常に固定値のため、ボタンとトリガーの一致を比較)
            return Buttons == compare.Buttons &&
                Triggers.Equals(compare.Triggers);
        }
    }
}

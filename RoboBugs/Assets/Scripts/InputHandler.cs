using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// This is an Input Handler Script Written by Patrick Emmons for the game "Pin Brawl" in 2024
/// </summary>
public class InputHandler : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public enum Inputs
    {
        Left,
        Right,
        Up,
        Down,
        Jump,
        Attack,
        Grapple,
        Shield,
        Menu,
        Pause,
    }
    public enum InputState
    {
        UnPressed,
        Pressed,
        Held,
        Released,
    }
    public Dictionary<Inputs, InputState> keyBindings = new Dictionary<Inputs, InputState>()
    {
        {Inputs.Left, InputState.UnPressed},
        {Inputs.Right, InputState.UnPressed},
        {Inputs.Up, InputState.UnPressed},
        {Inputs.Down, InputState.UnPressed},
        {Inputs.Jump, InputState.UnPressed},
        {Inputs.Attack, InputState.UnPressed},
        {Inputs.Grapple, InputState.UnPressed},
        {Inputs.Shield, InputState.UnPressed},
        {Inputs.Menu, InputState.UnPressed },
        {Inputs.Pause, InputState.UnPressed},
    };

    public Dictionary<Inputs, InputState> prevInputs = new Dictionary<Inputs, InputState>()
    {
        {Inputs.Left, InputState.UnPressed},
        {Inputs.Right, InputState.UnPressed},
        {Inputs.Up, InputState.UnPressed},
        {Inputs.Down, InputState.UnPressed},
        {Inputs.Jump, InputState.UnPressed},
        {Inputs.Attack, InputState.UnPressed},
        {Inputs.Grapple, InputState.UnPressed},
        {Inputs.Shield, InputState.UnPressed},
        {Inputs.Menu, InputState.UnPressed },
        {Inputs.Pause, InputState.UnPressed},
    };

    public void UpdateMovement(InputAction.CallbackContext context)
    {
        Vector2 movementVector = context.ReadValue<Vector2>();

        // Update left and right inputs
        if (movementVector.x < 0)
        {
            if (prevInputs[Inputs.Left] == InputState.UnPressed || prevInputs[Inputs.Left] == InputState.Released)
            {
                keyBindings[Inputs.Left] = InputState.Pressed;
            }
            else
            {
                keyBindings[Inputs.Left] = InputState.Held;
            }
            keyBindings[Inputs.Right] = InputState.UnPressed;
        }
        else if (movementVector.x > 0)
        {
            if (prevInputs[Inputs.Right] == InputState.UnPressed || prevInputs[Inputs.Right] == InputState.Released)
            {
                keyBindings[Inputs.Right] = InputState.Pressed;
            }
            else
            {
                keyBindings[Inputs.Right] = InputState.Held;
            }
            keyBindings[Inputs.Left] = InputState.UnPressed;
        }
        else
        {
            if (prevInputs[Inputs.Left] == InputState.Pressed || prevInputs[Inputs.Left] == InputState.Held)
            {
                keyBindings[Inputs.Left] = InputState.Released;
            }
            else
            {
                keyBindings[Inputs.Left] = InputState.UnPressed;
            }

            if (prevInputs[Inputs.Right] == InputState.Pressed || prevInputs[Inputs.Right] == InputState.Held)
            {
                keyBindings[Inputs.Right] = InputState.Released;
            }
            else
            {
                keyBindings[Inputs.Right] = InputState.UnPressed;
            }
        }

        // Update up and down inputs
        if (movementVector.y < 0)
        {
            if (prevInputs[Inputs.Down] == InputState.UnPressed || prevInputs[Inputs.Down] == InputState.Released)
            {
                keyBindings[Inputs.Down] = InputState.Pressed;
            }
            else
            {
                keyBindings[Inputs.Down] = InputState.Held;
            }
            keyBindings[Inputs.Up] = InputState.UnPressed;
        }
        else if (movementVector.y > 0)
        {
            if (prevInputs[Inputs.Up] == InputState.UnPressed || prevInputs[Inputs.Up] == InputState.Released)
            {
                keyBindings[Inputs.Up] = InputState.Pressed;
            }
            else
            {
                keyBindings[Inputs.Up] = InputState.Held;
            }
            keyBindings[Inputs.Down] = InputState.UnPressed;
        }
        else
        {
            if (prevInputs[Inputs.Down] == InputState.Pressed || prevInputs[Inputs.Down] == InputState.Held)
            {
                keyBindings[Inputs.Down] = InputState.Released;
            }
            else
            {
                keyBindings[Inputs.Down] = InputState.UnPressed;
            }

            if (prevInputs[Inputs.Up] == InputState.Pressed || prevInputs[Inputs.Up] == InputState.Held)
            {
                keyBindings[Inputs.Up] = InputState.Released;
            }
            else
            {
                keyBindings[Inputs.Up] = InputState.UnPressed;
            }
        }
    }

    public void UpdateButton(InputAction.CallbackContext context)
    {
        InputAction button = context.action;
        
        // Set button input states
        switch (button.name)
        {
            case "Jump":
                HandleButtonInput(context, Inputs.Jump);
                break;
            case "Attack":
                HandleButtonInput(context, Inputs.Attack);
                break;
            case "Grapple":
                HandleButtonInput(context, Inputs.Grapple);
                break;
            case "Shield":
                HandleButtonInput(context, Inputs.Shield);
                break;
            case "Menu":
                HandleButtonInput(context, Inputs.Menu);
                break;
            case "Pause":
                HandleButtonInput(context, Inputs.Pause);
                break;
        }
    }

    private void HandleButtonInput(InputAction.CallbackContext context, Inputs input)
    {
        if (context.performed)
        {
            if (prevInputs[input] == InputState.UnPressed || prevInputs[input] == InputState.Released)
            {
                Debug.Log($"{input} Pressed");
                keyBindings[input] = InputState.Pressed;
            }
            else
            {
                keyBindings[input] = InputState.Held;
            }
        }
        else
        {
            if (prevInputs[input] == InputState.Pressed || prevInputs[input] == InputState.Held)
            {
                keyBindings[input] = InputState.Released;
            }
            else
            {
                keyBindings[input] = InputState.UnPressed;
            }
        }
    }


    public void FixedUpdate()
    {
        List<Inputs> keys = new List<Inputs>(keyBindings.Keys);
        foreach (Inputs key in keys)
        {
            prevInputs[key] = keyBindings[key];
            if (keyBindings[key] == InputState.Pressed)
            {
                keyBindings[key] = InputState.Held;
            }
            if (keyBindings[key] == InputState.Released)
            {
                keyBindings[key] = InputState.UnPressed;
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    // Singleton pattern to ensure only one instance exists
    private static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("InputManager");
                    _instance = obj.AddComponent<InputManager>();
                }
            }
            return _instance;
        }
    }

    // Event to be triggered when the key is pressed
    public delegate void OnKeyPressed(KeyCode keyCode);
    public event OnKeyPressed KeyPressed;

    private void Update()
    {
        // Call the method to get the pressed keys
        if (Input.anyKeyDown)
        {
            KeyCode[] pressedKeys = GetPressedKeys();
            foreach (var key in pressedKeys)
            {
                KeyPressed?.Invoke(key);
            }
        }
    }

    KeyCode[] GetPressedKeys()
    {
        List<KeyCode> pressedKeys = new List<KeyCode>();

        // Iterate through all keys
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            // Check if the current key is pressed
            if (Input.GetKey(keyCode))
            {
                // Return the pressed key
                pressedKeys.Add(keyCode);
            }
        }

        // Convert the list to an array
        return pressedKeys.ToArray();
    }
}
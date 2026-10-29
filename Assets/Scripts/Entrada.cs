using UnityEngine;
using UnityEngine.InputSystem;

// Lee el teclado con el Input System de Unity 6.
// Asi todos los scripts preguntan aqui y no repiten codigo.
public static class Entrada
{

    public static float Horizontal()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return 0f;

        float x = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
        return x;
    }

    public static bool SaltoPresionado()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return false;
        return k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame;
    }

    public static bool SaltoSoltado()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return false;
        return k.spaceKey.wasReleasedThisFrame || k.wKey.wasReleasedThisFrame || k.upArrowKey.wasReleasedThisFrame;
    }

    // Q: Contener la luz (todo alrededor se congela)
    public static bool Contener()
    {
        Keyboard k = Keyboard.current;
        return k != null && k.qKey.isPressed;
    }

    public static bool ContenerPresionado()
    {
        Keyboard k = Keyboard.current;
        return k != null && k.qKey.wasPressedThisFrame;
    }

    // SHIFT: Destello (libera todo por un momento)
    public static bool Destello()
    {
        Keyboard k = Keyboard.current;
        return k != null && (k.leftShiftKey.wasPressedThisFrame || k.rightShiftKey.wasPressedThisFrame);
    }

    public static bool Reiniciar()
    {
        Keyboard k = Keyboard.current;
        return k != null && k.rKey.wasPressedThisFrame;
    }

    public static bool Continuar()
    {
        Keyboard k = Keyboard.current;
        return k != null && (k.spaceKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame);
    }
}

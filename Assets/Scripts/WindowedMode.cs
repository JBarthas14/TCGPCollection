using System.Diagnostics;
using UnityEngine;

public class WindowedMode : MonoBehaviour
{
    void Start()
    {
        // Désactiver le fullscreen et définir la résolution
        Screen.SetResolution(1440, 810, false);
    }

    void Ondestroy()
    {
       Application.Quit();
       Process.GetCurrentProcess().Kill(); 
    }
}
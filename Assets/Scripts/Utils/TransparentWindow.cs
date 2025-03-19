using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meowdieval.Core.Utils
{
    public class TransparentWindow : MonoBehaviour
    {
#if !UNITY_EDITOR
            // Import Windows API functions
            [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
            [DllImport("user32.dll", SetLastError = true)] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
            [DllImport("user32.dll", SetLastError = true)] private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
            [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
            [DllImport("user32.dll", SetLastError = true)] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            private const int GWL_EXSTYLE = -20;
            private const uint WS_EX_LAYERED = 0x00080000;
            private const uint WS_EX_TRANSPARENT = 0x00000020;
            private const uint LWA_COLORKEY = 0x00000001;
            private const uint LWA_ALPHA = 0x00000002;
            private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

            public event Action<bool> OnClickthroughChanged;

            private void Start()
            {
                // Get the current window handle
                IntPtr hWnd = GetActiveWindow();

                // Set the window to always be on top
                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);

                // Set the window to be transparent
                uint windowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
                SetWindowLong(hWnd, GWL_EXSTYLE, windowLong | WS_EX_LAYERED | WS_EX_TRANSPARENT);
                // Set the background to be transparent
                SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);
            }

            private void Update()
            {
                bool isOverUI = IsPointerOverUIElement();
                SetClickthrough(!isOverUI);
            }

            private bool IsPointerOverUIElement()
            {
                // Check for 2D UI elements
                if (Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) != null)
                    return true;

                // Check for 3D UI elements
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                    return hit.collider != null;

                return false;
            }

            private void SetClickthrough(bool clickthrough)
            {
                if (clickthrough)
                {
                    SetWindowLong(GetActiveWindow(), GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
                }
                else
                {
                    SetWindowLong(GetActiveWindow(), GWL_EXSTYLE, WS_EX_LAYERED);
                }

                OnClickthroughChanged?.Invoke(clickthrough);
            }
#endif
    }
}
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meowdieval.Core.Utils
{
    /// <summary>
    /// This class makes the Unity window transparent and ensures it is always on top.
    /// It also handles click-through functionality based on whether the pointer is over a UI element.
    /// </summary>
    public class TransparentWindow : MonoBehaviour
    {
        // Import Windows API functions
        [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll", SetLastError = true)] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll", SetLastError = true)] private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("user32.dll", SetLastError = true)] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int GWL_EXSTYLE = -20;
        private const uint WS_EX_LAYERED = 0x00080000;
        private const uint WS_EX_TRANSPARENT = 0x00000020;
        private const uint LWA_COLORKEY = 0x00000001;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        public event Action<bool> OnClickthroughChanged;

        private bool isClickthrough = false;
        private IntPtr hWnd;

        /// <summary>
        /// Initializes the window settings to make it transparent and always on top.
        /// </summary>
        private void Start()
        {
            // Get the current window handle
            hWnd = GetActiveWindow();

            // Set the window to always be on top
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);

            // Set the window to be transparent
            uint windowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, windowLong | WS_EX_LAYERED);

            // Set the background to be transparent
            SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

            // Ensure the window is in focus
            SetForegroundWindow(hWnd);
        }

        /// <summary>
        /// Updates the click-through state based on whether the pointer is over a UI element.
        /// Ensures the window remains in focus when clicked.
        /// </summary>
        private void Update()
        {
            bool isOverElement = IsPointerOverElement();
            SetClickthrough(!isOverElement);

            // Ensure the window regains focus when an element is clicked
            if (Input.GetMouseButtonDown(0) && isOverElement)
            {
                SetForegroundWindow(hWnd);
            }
        }

        /// <summary>
        /// Checks if the pointer is over a UI element.
        /// </summary>
        /// <returns>True if the pointer is over a UI element, otherwise false.</returns>
        private bool IsPointerOverElement()
        {
            // Check for 2D UI elements
            if (Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) != null)
            {
                return true;
            }

            // Check for 3D UI elements
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the click-through state of the window.
        /// </summary>
        /// <param name="clickthrough">True to enable click-through, false to disable.</param>
        private void SetClickthrough(bool clickthrough)
        {
            if (clickthrough == isClickthrough) return;

            isClickthrough = clickthrough;
            uint exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

            if (clickthrough)
            {
                exStyle |= WS_EX_TRANSPARENT;
            }
            else
            {
                exStyle &= ~WS_EX_TRANSPARENT;
            }

            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
            OnClickthroughChanged?.Invoke(clickthrough);
        }
    }
}

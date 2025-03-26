using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meowdieval.Core.Utils
{
    /// <summary>
    /// Handles advanced window management for Unity applications including:
    /// - Transparent click-through windows
    /// - Always-on-top functionality
    /// </summary>
    public class TransparentWindow : MonoBehaviour
    {
        #region Windows API Imports

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        #endregion

        #region Constants and Structs

        private const int GWL_EXSTYLE = -20;
        private const uint WS_EX_LAYERED = 0x00080000;
        private const uint WS_EX_TRANSPARENT = 0x00000020;
        private const uint LWA_COLORKEY = 0x00000001;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public override string ToString() => $"({X}, {Y})";
        }

        #endregion

        #region Fields and Events

        public event Action<bool> OnClickthroughChanged;

        private IntPtr hWnd;
        private bool isClickthrough = false;
        private bool isDragging = false;
        private POINT lastMousePosition;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            if (Application.isEditor) return;

            hWnd = GetActiveWindow();
            InitializeWindow();
        }

        private void Update()
        {
            if (Application.isEditor) return;

            UpdateClickthroughState();
            HandleWindowDragging();
            MaintainForegroundState();
        }

        #endregion

        #region Window Configuration

        private void InitializeWindow()
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetWindowStyle(WS_EX_LAYERED);
            SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);
            SetForegroundWindow(hWnd);
        }

        private void SetWindowStyle(uint style)
        {
            uint currentStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, currentStyle | style);
        }

        #endregion

        #region Input Handling

        private void UpdateClickthroughState()
        {
            bool isOverElement = IsPointerOverUIElement();
            SetClickthrough(!isOverElement);
        }

        private void HandleWindowDragging()
        {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
            {
                StartDragging();
            }

            if (isDragging)
            {
                UpdateWindowPosition();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        private void StartDragging()
        {
            isDragging = true;
            GetCursorPos(out lastMousePosition);
        }

        private void UpdateWindowPosition()
        {
            POINT currentPos;
            GetCursorPos(out currentPos);

            int deltaX = currentPos.X - lastMousePosition.X;
            int deltaY = currentPos.Y - lastMousePosition.Y;

            SetWindowPos(hWnd, IntPtr.Zero, deltaX, deltaY, 0, 0, SWP_SHOWWINDOW | SWP_NOSIZE | SWP_NOZORDER);

            lastMousePosition = currentPos;
        }

        #endregion

        #region Utility Methods

        private bool IsPointerOverUIElement()
        {
            return Check2DCollision() || Check3DCollision();
        }

        private bool Check2DCollision()
        {
            return Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) != null;
        }

        private bool Check3DCollision()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out RaycastHit hit) && hit.collider != null;
        }

        private void MaintainForegroundState()
        {
            if (Input.GetMouseButtonDown(0) && IsPointerOverUIElement())
            {
                SetForegroundWindow(hWnd);
            }
        }

        private void SetClickthrough(bool enable)
        {
            if (enable == isClickthrough) return;

            isClickthrough = enable;
            UpdateWindowTransparency();
            OnClickthroughChanged?.Invoke(enable);
        }

        private void UpdateWindowTransparency()
        {
            uint exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            exStyle = isClickthrough ?
                exStyle | WS_EX_TRANSPARENT :
                exStyle & ~WS_EX_TRANSPARENT;

            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
        }

        #endregion
    }
}

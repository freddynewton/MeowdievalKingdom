using UnityEngine;
using UnityEngine.UI;

namespace Meowdieval.Core.Utils
{
    /// <summary>
    /// This class allows dragging a game object to follow the mouse cursor along the X and Z axes.
    /// </summary>
    public class WindowEnvironmentDragger : MonoBehaviour
    {
        [SerializeField] private Transform _gameLevel;
        [SerializeField] private RectTransform _gameLevelUi;
        [SerializeField] private Button _dragButton;

        [Header("Settings")]
        [SerializeField] private LayerMask worldPlaneLayer;
        [SerializeField] private float _smoothTime = 0.3f; // Time to smooth the movement

        private Camera _mainCamera;
        private bool _isDragging = false; // Flag to track the dragging status
        private Vector3 _velocity = Vector3.zero; // Velocity for SmoothDamp
        private Vector3 _offset;

        private void Start()
        {
            // Cache the main camera reference
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            // Start dragging when the mouse button is pressed and the drag button is selected
            if (Input.GetMouseButtonDown(0))
            {
                StartDragging();
            }

            // Move the object to follow the mouse cursor if dragging is active
            if (_isDragging)
            {
                MoveObjectToMousePosition();
            }

            // Stop dragging when the mouse button is released
            if (Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }

        /// <summary>
        /// Starts the dragging process.
        /// </summary>
        private void StartDragging()
        {
            // Get the mouse position in screen space
            Vector3 mouseScreenPosition = Input.mousePosition;

            // Create a ray from the camera to the mouse position
            Ray ray = _mainCamera.ScreenPointToRay(mouseScreenPosition);

            // Perform the raycast and check if it hits the "WorldPlane" layer
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, worldPlaneLayer))
            {
                _isDragging = true;

                _offset = _gameLevel.position - hit.point;
            }
        }

        /// <summary>
        /// Moves the object to the current mouse position, only along the X and Z axes.
        /// </summary>
        private void MoveObjectToMousePosition()
        {
            // Get the mouse position in screen space
            Vector3 mouseScreenPosition = Input.mousePosition;

            // Create a ray from the camera to the mouse position
            Ray ray = _mainCamera.ScreenPointToRay(mouseScreenPosition);

            // Perform the raycast and check if it hits the "WorldPlane" layer
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, worldPlaneLayer))
            {
                Vector3 offset = hit.point + _offset;

                // Get the hit point and maintain the current Y position of the object
                Vector3 targetPosition = new Vector3(offset.x, _gameLevel.position.y, offset.z);

                // Smoothly move the object to the target position with a bit of overshoot
                _gameLevel.position = Vector3.SmoothDamp(_gameLevel.position, targetPosition, ref _velocity, _smoothTime * Time.deltaTime);

                // Update the UI element position to follow the game object
                Vector3 screenPosition = _mainCamera.WorldToScreenPoint(_gameLevel.position);
                _gameLevelUi.position = screenPosition;
            }
        }

        /// <summary>
        /// Stops the dragging process.
        /// </summary>
        private void StopDragging()
        {
            _isDragging = false;
        }
    }
}

using UnityEngine;

namespace Meowdieval.Core.Utils
{
    /// <summary>
    /// This class allows dragging a game object to follow the mouse cursor along the X and Z axes.
    /// </summary>
    public class WindowEnvironmentDragger : MonoBehaviour
    {
        [SerializeField] private Rigidbody _gameLevel;

        [Header("Settings")]
        [SerializeField] private LayerMask worldPlaneLayer;
        [SerializeField] private float _smoothTime = 0.3f; // Time to smooth the movement

        private Camera _mainCamera;
        private bool _isDragging = false; // Flag to track the dragging status
        private Vector3 _velocity = Vector3.zero; // Velocity for SmoothDamp

        private void Start()
        {
            // Cache the main camera reference
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            // Start dragging when the mouse button is pressed
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
                // Get the hit point and maintain the current Y position of the object
                Vector3 targetPosition = new Vector3(hit.point.x, _gameLevel.position.y, hit.point.z);

                // Smoothly move the object to the target position with a bit of overshoot
                _gameLevel.MovePosition(Vector3.SmoothDamp(_gameLevel.position, targetPosition, ref _velocity, _smoothTime));
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

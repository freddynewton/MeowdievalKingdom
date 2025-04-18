using Meowdieval.Core.GridSystem;
using Meowdieval.Core.InputHandler;
using Meowdieval.Core.Ui;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Meowdieval.Core.Building
{
	public class BuildingSystem : MonoBehaviour
	{
		[Header("Building Settings")]
		[SerializeField] private BuildingMenuUi _buildingMenuUi;
		[SerializeField] private Placeable _placeablePrefab;
		[SerializeField] private float _placementUiOffset = 0.5f;
		[SerializeField] private float _dragDetectionRadius = 1.0f; // Radius to detect proximity to a Placeable

		private EnvironmentGrid _environmentGrid;
		private PlayerInputHandler _playerInputHandler;

		private List<Placeable> _placeables = new List<Placeable>();
		private Placeable _currentPlaceable;
		private bool _isDragging = false;
		private bool _isPlacing = false;

		// Events for placement lifecycle
		public event Action OnStartPlacement;
		public event Action OnStopPlacement;
		public event Action OnAcceptPlacement;

		[Inject]
		public void Initialize(EnvironmentGrid environmentGrid, PlayerInputHandler playerInputHandler)
		{
			Debug.Log($"Initializing BuildingSystem with EnvironmentGrid: {environmentGrid}, PlayerInputHandler: {playerInputHandler}");

			_environmentGrid = environmentGrid ?? throw new ArgumentNullException(nameof(environmentGrid));
			_playerInputHandler = playerInputHandler ?? throw new ArgumentNullException(nameof(playerInputHandler));
		}

		private void OnEnable()
		{
			if (_playerInputHandler == null)
			{
				Debug.LogError("PlayerInputHandler is not assigned in BuildingSystem.");
				return;
			}

			_playerInputHandler.Initialize();

			_playerInputHandler.AttackInputAction.performed += OnAttackActionPerformed;
			_playerInputHandler.AttackInputAction.canceled += OnAttackActionCanceled;
		}

		private void OnDisable()
		{
			if (_playerInputHandler == null)
			{
				Debug.LogError("PlayerInputHandler is not assigned in BuildingSystem.");
				return;
			}

			_playerInputHandler.AttackInputAction.performed -= OnAttackActionPerformed;
			_playerInputHandler.AttackInputAction.canceled -= OnAttackActionCanceled;
		}

		private void Update()
		{
			if (_isDragging)
			{
				DragCurrentPlaceable();
			}
		}

		/// <summary>
		/// Starts the placement process for a new Placeable.
		/// </summary>
		/// <param name="placeablePrefab">The Placeable prefab to place.</param>
		public void StartPlacement(Placeable placeablePrefab)
		{
			if (_isPlacing) return;

			_placeablePrefab = placeablePrefab;

			_currentPlaceable = Instantiate(_placeablePrefab, transform);

			_isPlacing = true;

			_environmentGrid.ShowGrid();
			_buildingMenuUi.SetHudVisibility(true);

			OnStartPlacement?.Invoke();
		}

		/// <summary>
		/// Stops the placement process and cleans up any temporary objects.
		/// </summary>
		public void StopPlacement()
		{
			if (!_isPlacing) return;

			_isPlacing = false;

			if (_currentPlaceable != null)
			{
				Destroy(_currentPlaceable.gameObject);
				_currentPlaceable = null;
			}

			_buildingMenuUi.SetHudVisibility(false);
			_environmentGrid.HideGrid();

			OnStopPlacement?.Invoke();
		}

		/// <summary>
		/// Called when the AttackAction is performed (button pressed).
		/// </summary>
		/// <param name="context">The input action context.</param>
		private void OnAttackActionPerformed(InputAction.CallbackContext context)
		{
			StartPlacement(_placeablePrefab);

			if (!_isDragging && _isPlacing)
			{
				CheckForDragStart();
			}
		}

		/// <summary>
		/// Called when the AttackAction is canceled (button released).
		/// </summary>
		/// <param name="context">The input action context.</param>
		private void OnAttackActionCanceled(InputAction.CallbackContext context)
		{
			if (_isDragging)
			{
				StopDragging();
			}
		}

		/// <summary>
		/// Checks if the mouse is close to a Placeable and starts dragging if the AttackAction is performed.
		/// </summary>
		private void CheckForDragStart()
		{
			Vector3 mouseWorldPosition = GetMouseWorldPosition();
			foreach (var placeable in _placeables)
			{
				if (Vector3.Distance(mouseWorldPosition, placeable.transform.position) <= _dragDetectionRadius)
				{
					StartDragging(placeable);
					break;
				}
			}
		}

		/// <summary>
		/// Starts dragging the specified Placeable.
		/// </summary>
		/// <param name="placeable">The Placeable to drag.</param>
		private void StartDragging(Placeable placeable)
		{
			_currentPlaceable = placeable;
			_isDragging = true;

			_environmentGrid.ShowGrid();
			_buildingMenuUi.SetHudVisibility(true);

			OnStartPlacement?.Invoke();
		}

		/// <summary>
		/// Drags the currently selected Placeable to follow the mouse position.
		/// </summary>
		private void DragCurrentPlaceable()
		{
			if (_currentPlaceable == null || _environmentGrid.LastHighlightedCell == null)
			{
				return;
			}

			_currentPlaceable.transform.position = _environmentGrid.LastHighlightedCell.Position;

			_buildingMenuUi.BuildingHudCanvasGroup.transform.position = Camera.main.WorldToScreenPoint(
				_currentPlaceable.transform.position + Vector3.up * _placementUiOffset
			);
		}

		/// <summary>
		/// Stops dragging the current Placeable.
		/// </summary>
		private void StopDragging()
		{
			_isDragging = false;

			if (_currentPlaceable != null)
			{
				_currentPlaceable = null;
			}

			_buildingMenuUi.SetHudVisibility(false);
			_environmentGrid.HideGrid();

			OnStopPlacement?.Invoke();
		}

		/// <summary>
		/// Accepts the placement of the current Placeable.
		/// </summary>
		public void AcceptPlacement()
		{
			if (_currentPlaceable == null)
			{
				return;
			}

			if (_environmentGrid.LastHighlightedCell != null && _environmentGrid.LastHighlightedCell.CurrentCellState != GridCellState.Occupied)
			{
				_environmentGrid.LastHighlightedCell.SetGridCellState(GridCellState.Occupied);
				_currentPlaceable.Initialize(_environmentGrid.LastHighlightedCell);
				_placeables.Add(_currentPlaceable);
				_currentPlaceable = null;

				OnAcceptPlacement?.Invoke();

				_buildingMenuUi.SetHudVisibility(false);
				_environmentGrid.HideGrid();
			}
		}

		/// <summary>
		/// Gets the world position of the mouse.
		/// </summary>
		/// <returns>The world position of the mouse.</returns>
		private Vector3 GetMouseWorldPosition()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				return hit.point;
			}

			return Vector3.zero;
		}
	}
}

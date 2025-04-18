using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Meowdieval.Core.GridSystem
{
	/// <summary>
	/// Manages the environment grid, including grid cell creation, visibility, and highlighting.
	/// </summary>
	public class EnvironmentGrid : MonoBehaviour
	{
		#region Serialized Fields

		[Header("Grid Settings")]
		[SerializeField] private GameObject _environment;
		[SerializeField] private LayerMask _environmentLayerMask;
		[SerializeField] private GridCell _gridCellPrefab;

		[SerializeField] private float _cellSize = 1f;
		[SerializeField] private float _margin = 0.1f;
		[SerializeField] private Vector3 _offset = Vector3.zero;

		[Header("Animation Settings")]
		[SerializeField] private float _cellDelayAnimation = 0.1f;

		[Header("Highlight Settings")]
		[SerializeField] private float _highlightInterval = 1f; // Interval in seconds

		#endregion

		#region Private Fields

		private Renderer _environmentRenderer;
		private Terrain _environmentTerrain;
		private GameObject _cellsParent;
		private bool _isGridVisible = false;

		private GridCell[,] _gridCells;
		private GridCellAnimationController _gridAnimationController;

		private GridCell _lastHighlightedCell;
		private float _nextHighlightTime;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the 2D array of grid cells.
		/// </summary>
		public GridCell[,] GridCells => _gridCells;

		/// <summary>
		/// Gets whether the grid is currently visible.
		/// </summary>
		public bool IsGridVisible => _isGridVisible;

		public GridCell LastHighlightedCell => _lastHighlightedCell;

		#endregion

		#region Unity Methods

		/// <summary>
		/// Initializes the environment grid and creates grid cells.
		/// </summary>
		private void Start()
		{
			_environmentRenderer ??= _environment.GetComponent<Renderer>();
			_environmentTerrain ??= _environment.GetComponent<Terrain>();
			CreateGridCells();

			// Initialize the GridAnimationController
			_gridAnimationController = new GridCellAnimationController(_gridCells, _cellDelayAnimation, _environmentLayerMask);
		}

		/// <summary>
		/// Updates the grid, including highlighting the closest cell to the mouse.
		/// </summary>
		private void Update()
		{
			// Check if it's time to highlight the closest cell
			if (Time.time >= _nextHighlightTime)
			{
				HighlightCellClosestToMouse();
				_nextHighlightTime = Time.time + _highlightInterval; // Schedule the next highlight
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Shows the grid by toggling the visibility of all grid cells.
		/// </summary>
		public void ShowGrid()
		{
			if (_isGridVisible)
			{
				return;
			}

			_gridAnimationController ??= new GridCellAnimationController(_gridCells, _cellDelayAnimation, _environmentLayerMask);

			StartCoroutine(_gridAnimationController.ToggleGrid(true));
			_isGridVisible = true;
		}

		/// <summary>
		/// Hides the grid by toggling the visibility of all grid cells.
		/// </summary>
		public void HideGrid()
		{
			if (!_isGridVisible)
			{
				return;
			}

			_gridAnimationController ??= new GridCellAnimationController(_gridCells, _cellDelayAnimation, _environmentLayerMask);

			StartCoroutine(_gridAnimationController.ToggleGrid(false));
			_isGridVisible = false;
		}

		public GridCell GetClosestCell(Vector3 position)
		{
			if (_gridCells == null || _gridCells.Length == 0)
			{
				Debug.LogWarning("Grid cells are not initialized.");
				return null;
			}

			int closestRow = Mathf.RoundToInt((position.x - _offset.x) / _cellSize) % _gridCells.GetLength(0);
			int closestCol = Mathf.RoundToInt((position.z - _offset.z) / _cellSize) % _gridCells.GetLength(1);

			// Ensure indices are within bounds
			closestRow = (closestRow + _gridCells.GetLength(0)) % _gridCells.GetLength(0);
			closestCol = (closestCol + _gridCells.GetLength(1)) % _gridCells.GetLength(1);

			GridCell closestCell = _gridCells[closestRow, closestCol];
			return closestCell;
		}


		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the position on the environment from a given sky position by casting a ray downward.
		/// </summary>
		/// <param name="skyPosition">The position in the sky to cast the ray from.</param>
		/// <returns>The position on the environment, or Vector3.zero if no hit is found.</returns>
		private Vector3 GetPositionFromSky(Vector3 skyPosition)
		{
			Ray ray = new Ray(skyPosition, Vector3.down);
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _environmentLayerMask))
			{
				return hit.point;
			}
			return Vector3.zero;
		}

		/// <summary>
		/// Creates the grid cells based on the environment bounds.
		/// </summary>
		private void CreateGridCells()
		{
			Bounds bounds = GetEnvironmentBounds();
			int rows = Mathf.CeilToInt((bounds.max.x - bounds.min.x - 2 * _margin) / _cellSize);
			int cols = Mathf.CeilToInt((bounds.max.z - bounds.min.z - 2 * _margin) / _cellSize);

			_gridCells = new GridCell[rows, cols];

			_cellsParent = new GameObject("GridCells");
			_cellsParent.transform.SetParent(transform);

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					float x = bounds.min.x + _margin + i * _cellSize + _offset.x;
					float z = bounds.min.z + _margin + j * _cellSize + _offset.z;
					Vector3 position = GetPositionFromSky(new Vector3(x, 0, z) + Vector3.up * 100);

					// Check if the position is within SamplePosition
					if (!NavMesh.SamplePosition(position, out NavMeshHit hit, _cellSize, NavMesh.AllAreas))
					{
						continue;
					}

					if (position == Vector3.zero)
					{
						continue;
					}

					GridCell cell = Instantiate(_gridCellPrefab, position, Quaternion.identity, _cellsParent.transform);
					cell.Initialize(position, new Vector3(_cellSize, _cellSize, _cellSize), false);

					_gridCells[i, j] = cell;
				}
			}
		}

		/// <summary>
		/// Highlights the grid cell closest to the mouse position.
		/// </summary>
		private void HighlightCellClosestToMouse()
		{
			if (!_isGridVisible)
			{
				return;
			}

			// Cast a ray from the mouse position
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _environmentLayerMask))
			{
				Vector3 hitPoint = hit.point;

				GridCell closestCell = GetClosestCell(hitPoint);

				// Highlight the closest cell
				if (closestCell != null)
				{
					// Reset the last highlighted cell
					_lastHighlightedCell?.SetGridCellState(GridCellState.Default);

					// Highlight the new cell
					closestCell.SetGridCellState(GridCellState.Highlighted);
					_lastHighlightedCell = closestCell;
				}
				else
				{
					Debug.LogWarning("Closest cell is null or already highlighted.");
				}
			}
			else
			{
				Debug.LogWarning("Raycast did not hit any object.");
			}
		}

		/// <summary>
		/// Gets the bounds of the environment based on its renderer or terrain.
		/// </summary>
		/// <returns>The bounds of the environment.</returns>
		private Bounds GetEnvironmentBounds()
		{
			if (_environmentRenderer != null)
			{
				return _environmentRenderer.bounds;
			}
			else if (_environmentTerrain != null)
			{
				return new Bounds(_environmentTerrain.transform.position + _environmentTerrain.terrainData.size / 2, _environmentTerrain.terrainData.size);
			}

			return new Bounds();
		}

		#endregion
	}
}

using UnityEngine;
using UnityEngine.AI;

namespace Meowdieval.Core.GridSystem
{
	public class EnvironmentGrid : MonoBehaviour
	{
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

		private Renderer _environmentRenderer;
		private Terrain _environmentTerrain;
		private GameObject _cellsParent;
		private bool _isGridVisible = false;

		private GridCell[,] _gridCells;
		private GridCellAnimationController _gridAnimationController;

		private GridCell _lastHighlightedCell;
		private float _nextHighlightTime;

		public GridCell[,] GridCells => _gridCells;
		public bool IsGridVisible => _isGridVisible;

		private void Start()
		{
			_environmentRenderer ??= _environment.GetComponent<Renderer>();
			_environmentTerrain ??= _environment.GetComponent<Terrain>();
			CreateGridCells();

			// Initialize the GridAnimationController
			_gridAnimationController = new GridCellAnimationController(_gridCells, _cellDelayAnimation, _environmentLayerMask);
		}

		private void Update()
		{
			// Check if it's time to highlight the closest cell
			if (Time.deltaTime >= _nextHighlightTime)
			{
				HighlightCellClosestToMouse();
				_nextHighlightTime = Time.deltaTime + _highlightInterval; // Schedule the next highlight
			}
		}

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

		public Vector3 GetPositionFromSky(Vector3 skyPosition)
		{
			Ray ray = new Ray(skyPosition, Vector3.down);
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _environmentLayerMask))
			{
				return hit.point;
			}
			return Vector3.zero;
		}

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

				// Find the closest grid cell to the hit point
				GridCell closestCell = null;
				float closestDistance = float.MaxValue;

				foreach (var cell in _gridCells)
				{
					if (cell == null) continue;

					float distance = Vector3.Distance(hitPoint, cell.Position);
					if (distance < closestDistance)
					{
						closestDistance = distance;
						closestCell = cell;
					}
				}

				// Highlight the closest cell
				if (closestCell != null && closestCell != _lastHighlightedCell)
				{
					// Reset the last highlighted cell
					if (_lastHighlightedCell != null)
					{
						_lastHighlightedCell.SetGridCellState(GridCellState.Default);
					}

					// Highlight the new cell
					closestCell.SetGridCellState(GridCellState.Highlighted);
					_lastHighlightedCell = closestCell;
				}
			}
		}
	}
}

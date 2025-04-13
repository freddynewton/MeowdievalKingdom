using DG.Tweening;
using System.Collections;
using UnityEngine;

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

		private Renderer _environmentRenderer;
		private GameObject _cellsParent;
		private bool _isGridVisible = false;

		private GridCell[,] _gridCells;
		private GridCellAnimationController _gridAnimationController;

		public GridCell[,] GridCells => _gridCells;
		public bool IsGridVisible => _isGridVisible;

		private void Start()
		{
			_environmentRenderer ??= _environment.GetComponent<Renderer>();
			CreateGridCells();

			// Initialize the GridAnimationController
			_gridAnimationController = new GridCellAnimationController(_gridCells, _cellDelayAnimation);
		}

		private Bounds GetEnvironmentBounds()
		{
			if (_environmentRenderer == null)
			{
				return new Bounds();
			}

			return _environmentRenderer.bounds;
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
					Vector3 position = new Vector3(x, 0, z);

					GridCell cell = Instantiate(_gridCellPrefab, position, Quaternion.identity, _cellsParent.transform);
					cell.Initialize(position, new Vector3(_cellSize, _cellSize, _cellSize), false);

					_gridCells[i, j] = cell;
				}
			}
		}

		public void ShowGrid()
		{
			if (_isGridVisible)
			{
				return;
			}

			StartCoroutine(_gridAnimationController.ShowGrid());
			_isGridVisible = true;
		}

		public void HideGrid()
		{
			if (!_isGridVisible)
			{
				return;
			}

			StartCoroutine(_gridAnimationController.HideGrid());
			_isGridVisible = false;
		}
	}
}

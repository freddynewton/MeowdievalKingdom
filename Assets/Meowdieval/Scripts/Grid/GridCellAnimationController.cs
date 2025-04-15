using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Meowdieval.Core.GridSystem
{
	public class GridCellAnimationController
	{
		private GridCell[,] _gridCells;
		private float _waveDelay;
		private LayerMask _layerMask;
		private Camera _camera;

		public GridCellAnimationController(GridCell[,] gridCells, float waveDelay, LayerMask layerMask)
		{
			_camera = Camera.main;
			_gridCells = gridCells;
			_waveDelay = waveDelay;
			_layerMask = layerMask;
		}

		/// <summary>
		/// Toggles the visibility of the grid cells starting from the center of the screen (using a raycast) and expanding outward.
		/// </summary>
		/// <param name="isVisible">True to show the grid, false to hide it.</param>
		public IEnumerator ToggleGrid(bool isVisible)
		{
			// Get the center of the screen
			Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

			// Cast a ray from the screen center
			Ray ray = _camera.ScreenPointToRay(screenCenter);
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
			{
				// Find the closest grid cell to the raycast hit point
				Vector3 hitPoint = hit.point;
				float closestDistance = float.MaxValue;
				int centerX = 0, centerY = 0;

				// Flatten the loop to improve performance by reducing nested iterations
				int rows = _gridCells.GetLength(0);
				int cols = _gridCells.GetLength(1);

				for (int i = 0; i < rows * cols; i++)
				{
					int x = i / cols;
					int y = i % cols;

					if (_gridCells[x, y] != null)
					{
						float distance = Vector3.Distance(hitPoint, _gridCells[x, y].Position);
						if (distance < closestDistance)
						{
							closestDistance = distance;
							centerX = x;
							centerY = y;
						}
					}
				}

				// Toggle the grid cells layer by layer outward from the center
				yield return ToggleLayerRecursive(centerX, centerY, 0, isVisible);
			}
		}

		/// <summary>
		/// Recursively toggles the visibility of grid cells layer by layer.
		/// </summary>
		/// <param name="centerX">The X index of the center grid cell.</param>
		/// <param name="centerY">The Y index of the center grid cell.</param>
		/// <param name="layer">The current layer to toggle.</param>
		/// <param name="isVisible">True to show the grid, false to hide it.</param>
		private IEnumerator ToggleLayerRecursive(int centerX, int centerY, int layer, bool isVisible)
		{
			bool hasCellsToToggle = false;

			for (int i = 0; i < _gridCells.GetLength(0); i++)
			{
				for (int j = 0; j < _gridCells.GetLength(1); j++)
				{
					int distanceX = Mathf.Abs(i - centerX);
					int distanceY = Mathf.Abs(j - centerY);

					if (distanceX + distanceY == layer && _gridCells[i, j] != null)
					{
						_gridCells[i, j].SetVisibility(isVisible);
						hasCellsToToggle = true;
					}
				}
			}

			if (hasCellsToToggle)
			{
				yield return new WaitForSeconds(_waveDelay);
				yield return ToggleLayerRecursive(centerX, centerY, layer + 1, isVisible);
			}
		}
	}
}

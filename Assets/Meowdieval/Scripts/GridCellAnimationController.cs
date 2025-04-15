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

		public GridCellAnimationController(GridCell[,] gridCells, float waveDelay, LayerMask layerMask)
		{
			_gridCells = gridCells;
			_waveDelay = waveDelay;
			_layerMask = layerMask;
		}

		/// <summary>
		/// Reveals the grid cells starting from the center of the screen (using a raycast) and expanding outward.
		/// </summary>
		public IEnumerator ShowGrid()
		{
			// Get the center of the screen
			Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

			// Cast a ray from the screen center
			Ray ray = Camera.main.ScreenPointToRay(screenCenter);
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
			{
				// Find the closest grid cell to the raycast hit point
				Vector3 hitPoint = hit.point;
				int centerX = 0, centerY = 0;
				float closestDistance = float.MaxValue;

				for (int i = 0; i < _gridCells.GetLength(0); i++)
				{
					for (int j = 0; j < _gridCells.GetLength(1); j++)
					{
						if (_gridCells[i, j] != null)
						{
							float distance = Vector3.Distance(hitPoint, _gridCells[i, j].Position);
							if (distance < closestDistance)
							{
								closestDistance = distance;
								centerX = i;
								centerY = j;
							}
						}
					}
				}

				// Reveal the grid cells layer by layer outward from the center
				yield return ShowLayerRecursive(centerX, centerY, 0);
			}
		}

		/// <summary>
		/// Hides the grid cells starting from the center of the screen (using a raycast) and expanding outward.
		/// </summary>
		public IEnumerator HideGrid()
		{
			// Get the center of the screen
			Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

			// Cast a ray from the screen center
			Ray ray = Camera.main.ScreenPointToRay(screenCenter);
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
			{
				// Find the closest grid cell to the raycast hit point
				Vector3 hitPoint = hit.point;
				int centerX = 0, centerY = 0;
				float closestDistance = float.MaxValue;

				for (int i = 0; i < _gridCells.GetLength(0); i++)
				{
					for (int j = 0; j < _gridCells.GetLength(1); j++)
					{
						if (_gridCells[i, j] != null)
						{
							float distance = Vector3.Distance(hitPoint, _gridCells[i, j].Position);
							if (distance < closestDistance)
							{
								closestDistance = distance;
								centerX = i;
								centerY = j;
							}
						}
					}
				}

				// Hide the grid cells layer by layer outward from the center
				yield return HideLayerRecursive(centerX, centerY, 0);
			}
		}

		private IEnumerator ShowLayerRecursive(int centerX, int centerY, int layer)
		{
			bool hasCellsToShow = false;

			for (int i = 0; i < _gridCells.GetLength(0); i++)
			{
				for (int j = 0; j < _gridCells.GetLength(1); j++)
				{
					int distanceX = Mathf.Abs(i - centerX);
					int distanceY = Mathf.Abs(j - centerY);

					if (distanceX + distanceY == layer && _gridCells[i, j] != null)
					{
						_gridCells[i, j].SetVisibility(true);
						hasCellsToShow = true;
					}
				}
			}

			if (hasCellsToShow)
			{
				yield return new WaitForSeconds(_waveDelay);
				yield return ShowLayerRecursive(centerX, centerY, layer + 1);
			}
		}

		private IEnumerator HideLayerRecursive(int centerX, int centerY, int layer)
		{
			bool hasCellsToHide = false;

			for (int i = 0; i < _gridCells.GetLength(0); i++)
			{
				for (int j = 0; j < _gridCells.GetLength(1); j++)
				{
					int distanceX = Mathf.Abs(i - centerX);
					int distanceY = Mathf.Abs(j - centerY);

					if (distanceX + distanceY == layer && _gridCells[i, j] != null)
					{
						_gridCells[i, j].SetVisibility(false);
						hasCellsToHide = true;
					}
				}
			}

			if (hasCellsToHide)
			{
				yield return new WaitForSeconds(_waveDelay);
				yield return HideLayerRecursive(centerX, centerY, layer + 1);
			}
		}
	}
}

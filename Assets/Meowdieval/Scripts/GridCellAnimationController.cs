using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Meowdieval.Core.GridSystem
{
	public class GridCellAnimationController
	{
		private GridCell[,] _gridCells;
		private float _waveDelay;

		public GridCellAnimationController(GridCell[,] gridCells, float waveDelay)
		{
			_gridCells = gridCells;
			_waveDelay = waveDelay;
		}

		public IEnumerator ShowGrid()
		{
			for (int i = 0; i < _gridCells.GetLength(0); i++)
			{
				for (int j = 0; j < _gridCells.GetLength(1); j++)
				{
					_gridCells[i, j].SetVisibility(true);
				}
				yield return new WaitForSeconds(_waveDelay);
			}
		}

		public IEnumerator HideGrid()
		{
			int centerX = _gridCells.GetLength(0) / 2;
			int centerY = _gridCells.GetLength(1) / 2;

			yield return HideLayerRecursive(centerX, centerY, 0);
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

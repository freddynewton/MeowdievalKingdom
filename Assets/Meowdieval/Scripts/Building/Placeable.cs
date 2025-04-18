using Meowdieval.Core.GridSystem;
using UnityEngine;

namespace Meowdieval.Core.Building
{
	public class Placeable : MonoBehaviour
	{
		private GridCell _currentCell;

		public void Initialize(GridCell cell)
		{
			_currentCell = cell;
			transform.position = cell.Position;
		}

		public void RemoveFromGrid()
		{
			if (_currentCell != null)
			{
				_currentCell.SetGridCellState(GridCellState.Default);
				_currentCell = null;
			}
		}
	}
}

using UnityEngine;

namespace Meowdieval.Core.Data
{
	/// <summary>
	/// ScriptableObject to hold color settings for grid cells.
	/// </summary>
	[CreateAssetMenu(fileName = "GridCellColorSettings", menuName = "Meowdieval/GridCellColorSettings")]
	public class GridCellSettings : ScriptableObject
	{
		/// <summary>
		/// The default color of a grid cell.
		/// </summary>
		public Color DefaultColor = Color.white;

		/// <summary>
		/// The color of an occupied grid cell.
		/// </summary>
		public Color OccupiedColor = Color.red;

		/// <summary>
		/// The color of a highlighted grid cell.
		/// </summary>
		public Color HighlightColor = Color.cyan;
	}
}

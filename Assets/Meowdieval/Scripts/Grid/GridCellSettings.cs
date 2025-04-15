using Meowdieval.Core.GridSystem;
using UnityEngine;

namespace Meowdieval.Core.Data
{
	/// <summary>
	/// ScriptableObject to hold color settings for grid cells.
	/// </summary>
	[CreateAssetMenu(fileName = "GridCellColorSettings", menuName = "Meowdieval/GridCellColorSettings")]
	public class GridCellSettings : ScriptableObject
	{
		[Header("Default Cell Settings")]
		[Range(0f, 1f)]
		[Tooltip("Alpha value for the default cell color.")]
		public float DefaultAlpha = 1f;

		/// <summary>
		/// The default color of a grid cell.
		/// </summary>
		public Color DefaultColor = Color.white;

		[Header("Occupied Cell Settings")]
		[Range(0f, 1f)]
		[Tooltip("Alpha value for the occupied cell color.")]
		public float OccupiedAlpha = 1f;

		/// <summary>
		/// The color of an occupied grid cell.
		/// </summary>
		public Color OccupiedColor = Color.red;

		[Header("Highlighted Cell Settings")]
		[Range(0f, 1f)]
		[Tooltip("Alpha value for the highlighted cell color.")]
		public float HighlightAlpha = 1f;

		/// <summary>
		/// The color of a highlighted grid cell.
		/// </summary>
		public Color HighlightColor = Color.cyan;

		/// <summary>
		/// Gets the color corresponding to the given grid cell state.
		/// </summary>
		/// <param name="state">The state of the grid cell.</param>
		/// <returns>The color associated with the given state.</returns>
		public Color GetColor(GridCellState state)
		{
			return state switch
			{
				GridCellState.Default => DefaultColor,
				GridCellState.Occupied => OccupiedColor,
				GridCellState.Highlighted => HighlightColor,
				_ => throw new System.ArgumentOutOfRangeException(nameof(state), $"Unhandled GridCellState: {state}")
			};
		}

		/// <summary>
		/// Gets the alpha value corresponding to the given grid cell state.
		/// </summary>
		/// <param name="state">The state of the grid cell.</param>
		/// <returns>The alpha value associated with the given state.</returns>
		public float GetAlpha(GridCellState state)
		{
			return state switch
			{
				GridCellState.Default => DefaultAlpha,
				GridCellState.Occupied => OccupiedAlpha,
				GridCellState.Highlighted => HighlightAlpha,
				_ => throw new System.ArgumentOutOfRangeException(nameof(state), $"Unhandled GridCellState: {state}")
			};
		}
	}
}

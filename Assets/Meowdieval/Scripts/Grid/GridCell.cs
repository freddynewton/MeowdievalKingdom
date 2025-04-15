using DG.Tweening;
using Meowdieval.Core.Data;
using UnityEngine;

namespace Meowdieval.Core.GridSystem
{
	/// <summary>
	/// Represents a single cell in the grid system. Handles its position, state, and visual appearance.
	/// </summary>
	public class GridCell : MonoBehaviour
	{
		[SerializeField] private GridCellSettings _colorSettings;

		/// <summary>
		/// Gets the position of the grid cell in world space.
		/// </summary>
		public Vector3 Position { get; private set; }

		/// <summary>
		/// Gets the current state of the grid cell.
		/// </summary>
		public GridCellState CurrentCellState { get; private set; }

		private MeshRenderer _meshRenderer;
		private Material _materialInstance;
		private Sequence _visibilitySequence;

		/// <summary>
		/// Initializes the grid cell by setting up its material and color.
		/// </summary>
		private void Awake()
		{
			// Get the MeshRenderer component from the child object.
			_meshRenderer ??= GetComponentInChildren<MeshRenderer>();

			// Create a material instance to avoid modifying the shared material.
			if (_meshRenderer != null)
			{
				_materialInstance = _meshRenderer.material;
			}

			// Set the default color of the grid cell.
			SetGridCellState(GridCellState.Default, 0);

			// Set the initial visibility of the grid cell.
			SetVisibility(false, 0, 0);
		}

		/// <summary>
		/// Initializes the grid cell with position, scale, and visibility.
		/// </summary>
		/// <param name="position">The world position of the grid cell.</param>
		/// <param name="scale">The scale of the grid cell.</param>
		/// <param name="isVisible">Whether the grid cell should be visible initially.</param>
		public void Initialize(Vector3 position, Vector3 scale, bool isVisible)
		{
			Position = position;
			transform.position = position;
			transform.localScale = scale;

			SetVisibility(isVisible, 0, 0);
		}

		/// <summary>
		/// Sets the grid cell's type and updates its appearance accordingly.
		/// </summary>
		/// <param name="state">The type of the grid cell.</param>
		/// <param name="duration">The duration of the transition animation.</param>
		public void SetGridCellState(GridCellState state, float duration = 0.3f)
		{
			if (state == CurrentCellState)
			{
				return;
			}

			CurrentCellState = state;

			// Update the material color based on the new state
			Color targetColor = _colorSettings.GetColor(state);
			targetColor.a = _colorSettings.GetAlpha(state);

			if (_materialInstance != null)
			{
				_materialInstance.DOKill();
				_materialInstance.DOColor(targetColor, duration);
			}
		}

		/// <summary>
		/// Sets the visibility of the grid cell, including fade and jump animations.
		/// </summary>
		/// <param name="visible">True to make the cell visible, false to hide it.</param>
		/// <param name="ease">The easing function for the jump animation.</param>
		/// <param name="duration">The duration of the visibility transition animation.</param>
		public void SetVisibility(bool visible, Ease ease = Ease.OutBack, float duration = 0.3f)
		{
			if (_materialInstance == null)
			{
				return;
			}

			// Get the current color and alpha based on the state
			Color targetColor = _colorSettings.GetColor(CurrentCellState);

			if (visible)
			{
				gameObject.SetActive(true);
			}

			if (duration == 0f)
			{
				_materialInstance.color = targetColor;
				if (!visible)
				{
					gameObject.SetActive(false);
				}
				return;
			}
			_materialInstance.DOKill();
			transform.DOKill();
			_visibilitySequence?.Kill();

			// Create a DoTween sequence for better control over animations
			_visibilitySequence = DOTween.Sequence();

			// Cache alpha values to avoid redundant calls
			float targetAlpha = _colorSettings.GetAlpha(CurrentCellState);
			float fadeInAlpha = targetAlpha + 0.4f;

			// Add fade-in animation to the sequence
			_visibilitySequence.Append(_materialInstance.DOFade(fadeInAlpha, duration * 0.5f));

			// Add fade-out animation to the sequence
			_visibilitySequence.Append(_materialInstance.DOFade(targetAlpha, duration * 0.5f));

			// Add a callback to deactivate the game object if not visible
			_visibilitySequence.OnComplete(() =>
			{
				if (!visible)
				{
					gameObject.SetActive(false);
				}
			});

			// Add jump animation to the sequence
			_visibilitySequence.Join(transform.DOJump(transform.position, 0.5f, 1, duration).SetEase(ease));
		}
	}
}

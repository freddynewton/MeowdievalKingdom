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
		/// Indicates whether the grid cell is currently occupied.
		/// </summary>
		public bool IsOccupied { get; private set; }

		private MeshRenderer _meshRenderer;
		private Material _materialInstance;
		private Color _currentColor;

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
				_currentColor = _materialInstance.color;
			}
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
			IsOccupied = false;

			SetVisibility(isVisible, 0f);
		}

		/// <summary>
		/// Sets the occupied state of the grid cell and updates its appearance.
		/// </summary>
		/// <param name="occupied">True if the cell is occupied, false otherwise.</param>
		/// <param name="duration">The duration of the transition animation.</param>
		public void SetOccupied(bool occupied, float duration = 0.3f)
		{
			IsOccupied = occupied;
			if (occupied)
			{
				SetOccupiedState(duration);
			}
			else
			{
				SetDefaultState(duration);
			}
		}

		/// <summary>
		/// Updates the position of the grid cell in world space.
		/// </summary>
		/// <param name="position">The new position of the grid cell.</param>
		public void SetPosition(Vector3 position)
		{
			Position = position;
			transform.position = position;
		}

		/// <summary>
		/// Highlights the grid cell by changing its color to the highlight color.
		/// </summary>
		/// <param name="duration">The duration of the color transition animation.</param>
		public void HighlightCell(float duration = 0.3f)
		{
			if (_materialInstance != null)
			{
				_materialInstance.DOKill();
				_currentColor = _colorSettings.HighlightColor;
				_materialInstance.DOColor(_currentColor, duration);
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

			if (visible)
			{
				gameObject.SetActive(true);
			}

			if (duration == 0f)
			{
				_materialInstance.color = visible ? _currentColor : new Color(_currentColor.r, _currentColor.g, _currentColor.b, 0f);
				return;
			}

			_materialInstance.DOKill();
			transform.DOKill();

			float targetAlpha = visible ? _currentColor.a : 0f;

			_materialInstance.DOFade(targetAlpha, duration).OnComplete(() =>
			{
				if (!visible)
				{
					gameObject.SetActive(false);
				}
			});
			transform.DOJump(transform.position, 0.5f, 1, duration).SetEase(ease);
		}

		/// <summary>
		/// Resets the grid cell to its default state by changing its color to the default color.
		/// </summary>
		/// <param name="duration">The duration of the color transition animation.</param>
		private void SetDefaultState(float duration = 0.3f)
		{
			if (_materialInstance != null)
			{
				_materialInstance.DOKill();
				_materialInstance.DOColor(_colorSettings.DefaultColor, duration).OnComplete(() =>
				{
					_currentColor = _colorSettings.DefaultColor;
				});
			}
		}

		/// <summary>
		/// Updates the grid cell to its occupied state by changing its color to the occupied color.
		/// </summary>
		/// <param name="duration">The duration of the color transition animation.</param>
		private void SetOccupiedState(float duration = 0.3f)
		{
			if (_materialInstance != null)
			{
				_materialInstance.DOKill();
				_materialInstance.DOColor(_colorSettings.OccupiedColor, duration).OnComplete(() =>
				{
					_currentColor = _colorSettings.OccupiedColor;
				});
			}
		}
	}
}

using DG.Tweening;
using Meowdieval.Core.Data;
using UnityEngine;

namespace Meowdieval.Core.GridSystem
{
	public class GridCell : MonoBehaviour
	{
		[SerializeField] private GridCellSettings _colorSettings;

		public Vector3 Position { get; private set; }
		public bool IsOccupied { get; private set; }

		private MeshRenderer _meshRenderer;
		private Material _materialInstance;
		private Color _currentColor;

		private void Awake()
		{
			_meshRenderer ??= GetComponentInChildren<MeshRenderer>();

			// Create a material instance to avoid modifying the shared material  
			if (_meshRenderer != null)
			{
				_materialInstance = _meshRenderer.material;
				_currentColor = _materialInstance.color;
			}
		}

		public void Initialize(Vector3 position, Vector3 scale, bool isVisible)
		{
			Position = position;
			transform.position = position;
			transform.localScale = scale;
			IsOccupied = false;

			SetVisibility(isVisible, 0f);
		}

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

		public void SetPosition(Vector3 position)
		{
			Position = position;
			transform.position = position;
		}

		public void HighlightCell(float duration = 0.3f)
		{
			if (_materialInstance != null)
			{
				_materialInstance.DOKill();
				_currentColor = _colorSettings.HighlightColor;
				_materialInstance.DOColor(_currentColor, duration);
			}
		}

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

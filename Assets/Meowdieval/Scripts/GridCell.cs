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
		private Vector3 _initialScale;

		private SpriteRenderer _spriteRenderer;

		private void Awake()
		{
			_spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
		}

		public void Initialize(Vector3 position, Vector3 scale, bool isVisible)
		{
			Position = position;
			transform.position = position;

			_initialScale = scale;
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
			_spriteRenderer.DOKill();

			_spriteRenderer.DOColor(_colorSettings.HighlightColor, duration);
		}

		public void SetVisibility(bool visible, float duration = 0.3f)
		{
			transform.DOScale(visible ? _initialScale : Vector3.zero, duration);
		}

		private void SetDefaultState(float duration = 0.3f)
		{
			_spriteRenderer.DOKill();

			_spriteRenderer.DOColor(_colorSettings.DefaultColor, duration);
		}

		private void SetOccupiedState(float duration = 0.3f)
		{
			_spriteRenderer.DOKill();

			_spriteRenderer.DOColor(_colorSettings.OccupiedColor, duration);
		}
	}
}

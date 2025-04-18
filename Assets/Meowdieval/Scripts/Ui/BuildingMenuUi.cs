using DG.Tweening;
using UnityEngine;

namespace Meowdieval.Core.Ui
{
	public class BuildingMenuUi : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _buildingMenuCanvasGroup;
		[SerializeField] private CanvasGroup _buildingHudCanvasGroup;
		[SerializeField] private CanvasGroup _buildingScrollviewCanvasGroup;

		public CanvasGroup BuildingMenuCanvasGroup { get { return _buildingMenuCanvasGroup; } }
		public CanvasGroup BuildingHudCanvasGroup { get { return _buildingHudCanvasGroup; } }
		public CanvasGroup BuildingScrollviewCanvasGroup { get { return _buildingScrollviewCanvasGroup; } }

		private void Awake()
		{
			SetHudVisibility(false);
			SetMenuVisibility(true);
			SetScrollViewVisibility(true);
		}

		public void SetHudVisibility(bool isVisible)
		{
			float targetAlpha = isVisible ? 1 : 0;
			_buildingHudCanvasGroup.DOFade(targetAlpha, 0.3f);
			_buildingHudCanvasGroup.interactable = isVisible;
			_buildingHudCanvasGroup.blocksRaycasts = isVisible;
		}

		public void SetMenuVisibility(bool isVisible)
		{
			float targetAlpha = isVisible ? 1 : 0;
			_buildingMenuCanvasGroup.DOFade(targetAlpha, 0.3f);
			_buildingMenuCanvasGroup.interactable = isVisible;
			_buildingMenuCanvasGroup.blocksRaycasts = isVisible;
		}

		public void SetScrollViewVisibility(bool isVisible)
		{
			float targetAlpha = isVisible ? 1 : 0;
			_buildingScrollviewCanvasGroup.DOFade(targetAlpha, 0.3f);
			_buildingScrollviewCanvasGroup.interactable = isVisible;
			_buildingScrollviewCanvasGroup.blocksRaycasts = isVisible;
		}
	}
}

using UnityEngine;

namespace Meowdieval.Core.Unit
{
	public class UnitAnimationController : MonoBehaviour
	{
		[SerializeField] private Animator _animator;

		private UnitMainManager _unitMainManager;

		public int _speedHash { get; private set; }

		public void Initialize(UnitMainManager unitMainManager)
		{
			_unitMainManager = unitMainManager;

			_speedHash = Animator.StringToHash("Speed");
		}

		private void Update()
		{
			if (_unitMainManager == null)
			{
				Debug.LogWarning("UnitMainManager is not assigned.");
				return;
			}

			if (_animator == null)
			{
				Debug.LogWarning("Animator is not assigned.");
				return;
			}

			// Normalize the speed to a range of 0 to 1
			float normalizedSpeed = Mathf.Clamp01(_unitMainManager.UnitNavigationController.CurrentSpeed / _unitMainManager.UnitNavigationController.MaxSpeed);

			_animator.SetFloat(_speedHash, normalizedSpeed);
		}
	}
}

using Meowdieval.Core.Unit;
using UnityEngine;

namespace Meowdieval
{
	public class UnitMainManager : MonoBehaviour
	{
		private UnitNavigationController _unitNavigationController;
		private UnitAnimationController _unitAnimationController;

		public UnitNavigationController UnitNavigationController => _unitNavigationController;
		public UnitAnimationController UnitAnimationController => _unitAnimationController;

		private void Awake()
		{
			_unitNavigationController ??= GetComponent<UnitNavigationController>();
			_unitAnimationController ??= GetComponent<UnitAnimationController>();

			_unitNavigationController.Initialize(this);
			_unitAnimationController.Initialize(this);
		}
	}
}

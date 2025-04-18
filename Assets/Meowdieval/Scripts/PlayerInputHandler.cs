using UnityEngine;
using UnityEngine.InputSystem;

namespace Meowdieval.Core.InputHandler
{
	public class PlayerInputHandler : MonoBehaviour
	{
		[SerializeField] private InputActionAsset _inputActionAsset;

		private InputActionMap _playerInputActionMap;
		private InputActionMap _uiInputActionMap;

		private InputAction _lookInputAction;
		private InputAction _moveInputAction;
		private InputAction _attackInputAction;

		private bool _isInitialized = false;

		public InputAction LookInputAction => _lookInputAction;
		public InputAction MoveInputAction => _moveInputAction;
		public InputAction AttackInputAction => _attackInputAction;

		private void Awake()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (_isInitialized)
			{
				return;
			}

			if (_inputActionAsset == null)
			{
				Debug.LogError("InputActionAsset is not assigned.");
				return;
			}

			_playerInputActionMap = _inputActionAsset.FindActionMap("Player");
			_uiInputActionMap = _inputActionAsset.FindActionMap("UI");

			_lookInputAction = _playerInputActionMap?.FindAction("Look");
			_moveInputAction = _playerInputActionMap?.FindAction("Move");
			_attackInputAction = _playerInputActionMap?.FindAction("Interact");

			_playerInputActionMap?.Enable();
			_uiInputActionMap?.Enable();
			_inputActionAsset.Enable();

			_isInitialized = true;
		}
	}
}

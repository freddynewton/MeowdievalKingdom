using Meowdieval.Core.Unit;
using UnityEngine;
using UnityEngine.AI;

namespace Meowdieval.Core.Unit
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class UnitNavigationController : MonoBehaviour
	{
		private NavMeshAgent _navMeshAgent;
		private Vector3 _targetPosition;
		private float _wanderRadius = 10f;
		private float _wanderTimer = 5f;
		private float _timer;
		private int _frameCounter = 0;
		private int _frameCheckInterval = 20;

		private UnitMainManager _unitMainManager;

		public float CurrentSpeed
		{
			get => _navMeshAgent.velocity.magnitude;
			private set { _navMeshAgent.velocity = _navMeshAgent.transform.forward * value; }
		}

		public float MaxSpeed
		{
			get => _navMeshAgent.speed;
			private set { _navMeshAgent.speed = value; }
		}

		public void Initialize(UnitMainManager unitMainManager)
		{
			_unitMainManager = unitMainManager;
			_navMeshAgent = GetComponent<NavMeshAgent>();

			_timer = _wanderTimer;
		}

		/// <summary>
		/// Sets the destination for the unit.
		/// </summary>
		/// <param name="position">The target position.</param>
		public void GoTo(Vector3 position)
		{
			NavMeshHit navHit;
			if (NavMesh.SamplePosition(position, out navHit, _wanderRadius, NavMesh.AllAreas))
			{
				_navMeshAgent.SetDestination(navHit.position);
			}
			else
			{
				Debug.LogWarning("Failed to find a valid NavMesh position for the target.");
			}
		}

		/// <summary>
		/// Stops the unit's movement.
		/// </summary>
		public void Stop()
		{
			_navMeshAgent.isStopped = true;
		}

		/// <summary>
		/// Resumes the unit's movement if it was stopped.
		/// </summary>
		public void Resume()
		{
			_navMeshAgent.isStopped = false;
		}

		/// <summary>
		/// Sets the parameters for wandering behavior.
		/// </summary>
		/// <param name="radius">The radius for wandering.</param>
		/// <param name="timer">The timer interval for wandering.</param>
		public void SetWanderParameters(float radius, float timer)
		{
			_wanderRadius = radius;
			_wanderTimer = timer;
		}

		/// <summary>
		/// Makes the unit wander to a random position within the specified radius.
		/// </summary>
		private void Wander()
		{
			_targetPosition = GetRandomPosition();
			_navMeshAgent.SetDestination(_targetPosition);
		}

		// Update is called once per frame
		private void Update()
		{
			_timer += Time.deltaTime;
			_frameCounter++;

			if (_timer >= _wanderTimer)
			{
				Wander();
				_timer = 0;
			}

			if (_frameCounter >= _frameCheckInterval)
			{
				CheckPathValidity();
				_frameCounter = 0;
				_frameCheckInterval = Random.Range(10, 30);
			}
		}

		/// <summary>
		/// Checks if the current path is still valid and sets a new destination if necessary.
		/// </summary>
		private void CheckPathValidity()
		{
			if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && !_navMeshAgent.hasPath)
			{
				Wander();
			}
		}

		/// <summary>
		/// Gets a random position within the specified radius.
		/// </summary>
		/// <returns>A random position within the radius.</returns>
		private Vector3 GetRandomPosition()
		{
			Vector3 randomDirection = Random.insideUnitSphere * _wanderRadius;
			randomDirection += transform.position;
			NavMeshHit navHit;
			NavMesh.SamplePosition(randomDirection, out navHit, _wanderRadius, NavMesh.AllAreas);
			return navHit.position;
		}
	}
}

using UnityEngine;

namespace Meowdieval.Core.Ai
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class UnitNavigationController : MonoBehaviour
    {
        private UnityEngine.AI.NavMeshAgent _navMeshAgent;
        private Vector3 _targetPosition;
        private float _wanderRadius = 10f;
        private float _wanderTimer = 5f;
        private float _timer;
        private int _frameCounter = 0;
        private int _frameCheckInterval = 20;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _timer = _wanderTimer;
        }

        // Update is called once per frame
        void Update()
        {
            _timer += Time.deltaTime;
            _frameCounter++;

            if (_timer >= _wanderTimer)
            {
                _targetPosition = GetRandomPosition();
                _navMeshAgent.SetDestination(_targetPosition);
                _timer = 0;
            }

            // Check every 20 frames if the path is still valid
            if (_frameCounter >= _frameCheckInterval)
            {
                if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && !_navMeshAgent.hasPath)
                {
                    _targetPosition = GetRandomPosition();
                    _navMeshAgent.SetDestination(_targetPosition);
                }
                _frameCounter = 0;

                _frameCheckInterval = Random.Range(10, 30);
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
            UnityEngine.AI.NavMeshHit navHit;
            UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, _wanderRadius, -1);
            return navHit.position;
        }
    }
}

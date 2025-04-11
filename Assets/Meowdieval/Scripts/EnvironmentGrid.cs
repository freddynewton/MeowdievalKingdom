using UnityEngine;

namespace Meowdieval
{
    public class EnvironmentGrid : MonoBehaviour
    {
        [SerializeField] private GameObject _environment;
        [SerializeField] private LayerMask _environmentLayerMask;

        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private float _margin = 0.1f;
        [SerializeField] private Vector3 _offset = Vector3.zero;

        private Vector3[,] _cellPositions;
        private Renderer _environmentRenderer;

        public Vector3[,] CellPositions => _cellPositions;

        private Vector3[,] GetCellPositions()
        {
            Bounds bounds = GetEnvironmentBounds();
            int rows = Mathf.CeilToInt((bounds.max.x - bounds.min.x - 2 * _margin) / _cellSize);
            int cols = Mathf.CeilToInt((bounds.max.z - bounds.min.z - 2 * _margin) / _cellSize);
            Vector3[,] cellPositions = new Vector3[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    float x = bounds.min.x + _margin + i * _cellSize + _offset.x;
                    float z = bounds.min.z + _margin + j * _cellSize + _offset.z;
                    Vector3 position = new Vector3(x, bounds.max.y + 1 + _offset.y, z);
                    if (IsPositionValid(position))
                    {
                        cellPositions[i, j] = new Vector3(x, bounds.min.y + _offset.y, z);
                    }
                    else
                    {
                        cellPositions[i, j] = Vector3.zero;
                    }
                }
            }

            return cellPositions;
        }

        private bool IsPositionValid(Vector3 position)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, _environmentLayerMask))
            {
                if (hit.collider.gameObject == _environment)
                {
                    return true;
                }
            }
            return false;
        }

        private Bounds GetEnvironmentBounds()
        {
            if (_environmentRenderer == null)
            {
                return new Bounds();
            }

            return _environmentRenderer.bounds;
        }

        private void Start()
        {
            _environmentRenderer ??= _environment.GetComponent<Renderer>();
            _cellPositions = GetCellPositions();
        }

        private void OnDrawGizmos()
        {
            return;

            _environmentRenderer ??= _environment.GetComponent<Renderer>();
            _cellPositions = GetCellPositions();

            Gizmos.color = Color.green;
            foreach (Vector3 position in _cellPositions)
            {
                if (position != Vector3.zero)
                {
                    Gizmos.DrawWireCube(position, new Vector3(_cellSize, _cellSize, _cellSize));
                }
            }
        }
    }
}

using UnityEngine;

namespace Meowdieval
{
    public class ButtonTest : MonoBehaviour
    {
        private Renderer cubeRenderer;
        private Quaternion targetRotation;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            cubeRenderer = GetComponent<Renderer>();
            targetRotation = transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        ChangeColor();
                        SetRandomRotation();
                    }
                }
            }

            LerpRotation();
        }

        void ChangeColor()
        {
            cubeRenderer.material.color = new Color(Random.value, Random.value, Random.value);
        }

        void SetRandomRotation()
        {
            targetRotation = Random.rotation;
        }

        void LerpRotation()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
        }
    }
}

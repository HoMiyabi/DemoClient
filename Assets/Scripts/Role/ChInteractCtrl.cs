using UnityEngine;
using UnityEngine.InputSystem;

namespace Kirara
{
    public class ChInteractCtrl : MonoBehaviour
    {
        public float interactRadius = 3f;
        public float interactAngle = 60f;

        private IInteractable selected;

        private readonly Collider[] colliders = new Collider[32];

        private void Start()
        {
            PlayerSystem.Instance.input.Combat.Interact.started += InteractStarted;
        }

        private void OnDestroy()
        {
            PlayerSystem.Instance.input.Combat.Interact.started -= InteractStarted;
        }

        private void Update()
        {
            UpdateSelected();
        }

        private void UpdateSelected()
        {
            IInteractable newSelected = null;
            float newAngle = float.MaxValue;

            int size = Physics.OverlapSphereNonAlloc(transform.position, interactRadius, colliders);
            for (int i = 0; i < size; i++)
            {
                var col = colliders[i];
                if (!col.TryGetComponent(out IInteractable interactable)) continue;

                float angle = Vector3.Angle(transform.forward, col.transform.position - transform.position);

                if (angle > interactAngle) continue;

                if (angle < newAngle)
                {
                    newSelected = interactable;
                    newAngle = angle;
                }
            }
            if (newSelected != selected)
            {
                if (selected != null)
                {
                    selected.IsSelected = false;
                }
                selected = newSelected;
                if (selected != null)
                {
                    selected.IsSelected = true;
                }
            }
        }

        private void InteractStarted(InputAction.CallbackContext ctx)
        {
            selected?.Interact(transform);
        }
    }
}
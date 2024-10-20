using _ScreamJam.Scripts.Agent;
using _ScreamJam.Scripts.Scriptables;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace _ScreamJam.Scripts.Player
{
    //mostly copy pasta from Kinematic character controller
    public class Player : MonoBehaviour
    { 
        public CharacterController Character;
        public CharacterCamera CharacterCamera;
        public GameObjectListSO GoblinList;

        //inputs
        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
            GoblinList.Add(Character.gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
            HandleCallGoblins();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (Input.GetMouseButtonDown(1))
            {
                CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
            if (Character.HasWeapon)
            {
                characterInputs.Attack = Input.GetMouseButtonDown(0);
                characterInputs.ThrowWeapon = Input.GetMouseButtonDown(1);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Interact();
            }

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
        void HandleCallGoblins()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                var list = GoblinList.Value;
                var characterGO = Character.gameObject;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != characterGO)
                    {
                        var agent = list[i].GetComponent<GoblinAgent>();
                        agent.FollowPlayer(i, characterGO);
                    }
                }
            }
        }

        public void Interact()
        {
            var cam = CharacterCamera.Camera;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.SphereCast(ray.origin, 1.5f, ray.direction, out var hit))
            {
                var pickup = hit.transform.GetComponent<Weapon>();
                if (pickup != null)
                    Pickup(pickup);
                else
                {
                    var interactable = hit.transform.GetComponent<IInteractable>();
                    if (interactable != null)
                        interactable.Interact();    
                }
            }
        }
        
        //interactions
        public void Pickup(Weapon weapon)
        {
            if (!Character.HasWeapon)
            {
                Character.Pickup(weapon);
            }
        }
        
    }
}
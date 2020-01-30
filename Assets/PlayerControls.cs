// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Controller"",
            ""id"": ""4b858eec-6617-4138-a0ba-5ba033c939ba"",
            ""actions"": [
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""942bccd6-3702-48c8-9925-10a66ad7efed"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""JumpAction"",
                    ""type"": ""Button"",
                    ""id"": ""d3edf8df-e01e-48d1-9b02-5892ab54a15b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot/Flashlight"",
                    ""type"": ""Button"",
                    ""id"": ""6c1e8879-98c0-451c-a728-edd01cb5514d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""22d190b2-f97c-404a-856d-cf15141bd6e7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Flashlight Aim"",
                    ""type"": ""Button"",
                    ""id"": ""9b732177-6a35-4508-824c-08827dca5891"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""8252c816-879f-4c4f-8d4a-23c0e43890ef"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuitGame"",
                    ""type"": ""Button"",
                    ""id"": ""6d49ed2b-6dd4-48fb-9581-ebabd99411b4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4e294132-c2e1-4428-a2dd-039786de7adf"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49143f43-d038-40f6-92d2-0784fbb96f15"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""JumpAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a90bf8a9-f269-4987-93ae-c0715ea19360"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot/Flashlight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef2623f2-89b9-4442-9960-e00fddde34dd"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f28a8831-0cc9-4b69-be27-b7191b52e82f"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Flashlight Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc166c4c-8adb-464b-8b86-6a56c29f9e34"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc9cf7de-17b9-43b0-99ac-d04a44612b12"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuitGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Controller
        m_Controller = asset.FindActionMap("Controller", throwIfNotFound: true);
        m_Controller_Interact = m_Controller.FindAction("Interact", throwIfNotFound: true);
        m_Controller_JumpAction = m_Controller.FindAction("JumpAction", throwIfNotFound: true);
        m_Controller_ShootFlashlight = m_Controller.FindAction("Shoot/Flashlight", throwIfNotFound: true);
        m_Controller_Move = m_Controller.FindAction("Move", throwIfNotFound: true);
        m_Controller_FlashlightAim = m_Controller.FindAction("Flashlight Aim", throwIfNotFound: true);
        m_Controller_Sprint = m_Controller.FindAction("Sprint", throwIfNotFound: true);
        m_Controller_QuitGame = m_Controller.FindAction("QuitGame", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Controller
    private readonly InputActionMap m_Controller;
    private IControllerActions m_ControllerActionsCallbackInterface;
    private readonly InputAction m_Controller_Interact;
    private readonly InputAction m_Controller_JumpAction;
    private readonly InputAction m_Controller_ShootFlashlight;
    private readonly InputAction m_Controller_Move;
    private readonly InputAction m_Controller_FlashlightAim;
    private readonly InputAction m_Controller_Sprint;
    private readonly InputAction m_Controller_QuitGame;
    public struct ControllerActions
    {
        private @PlayerControls m_Wrapper;
        public ControllerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Interact => m_Wrapper.m_Controller_Interact;
        public InputAction @JumpAction => m_Wrapper.m_Controller_JumpAction;
        public InputAction @ShootFlashlight => m_Wrapper.m_Controller_ShootFlashlight;
        public InputAction @Move => m_Wrapper.m_Controller_Move;
        public InputAction @FlashlightAim => m_Wrapper.m_Controller_FlashlightAim;
        public InputAction @Sprint => m_Wrapper.m_Controller_Sprint;
        public InputAction @QuitGame => m_Wrapper.m_Controller_QuitGame;
        public InputActionMap Get() { return m_Wrapper.m_Controller; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ControllerActions set) { return set.Get(); }
        public void SetCallbacks(IControllerActions instance)
        {
            if (m_Wrapper.m_ControllerActionsCallbackInterface != null)
            {
                @Interact.started -= m_Wrapper.m_ControllerActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_ControllerActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_ControllerActionsCallbackInterface.OnInteract;
                @JumpAction.started -= m_Wrapper.m_ControllerActionsCallbackInterface.OnJumpAction;
                @JumpAction.performed -= m_Wrapper.m_ControllerActionsCallbackInterface.OnJumpAction;
                @JumpAction.canceled -= m_Wrapper.m_ControllerActionsCallbackInterface.OnJumpAction;
                @ShootFlashlight.started -= m_Wrapper.m_ControllerActionsCallbackInterface.OnShootFlashlight;
                @ShootFlashlight.performed -= m_Wrapper.m_ControllerActionsCallbackInterface.OnShootFlashlight;
                @ShootFlashlight.canceled -= m_Wrapper.m_ControllerActionsCallbackInterface.OnShootFlashlight;
                @Move.started -= m_Wrapper.m_ControllerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_ControllerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_ControllerActionsCallbackInterface.OnMove;
                @FlashlightAim.started -= m_Wrapper.m_ControllerActionsCallbackInterface.OnFlashlightAim;
                @FlashlightAim.performed -= m_Wrapper.m_ControllerActionsCallbackInterface.OnFlashlightAim;
                @FlashlightAim.canceled -= m_Wrapper.m_ControllerActionsCallbackInterface.OnFlashlightAim;
                @Sprint.started -= m_Wrapper.m_ControllerActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_ControllerActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_ControllerActionsCallbackInterface.OnSprint;
                @QuitGame.started -= m_Wrapper.m_ControllerActionsCallbackInterface.OnQuitGame;
                @QuitGame.performed -= m_Wrapper.m_ControllerActionsCallbackInterface.OnQuitGame;
                @QuitGame.canceled -= m_Wrapper.m_ControllerActionsCallbackInterface.OnQuitGame;
            }
            m_Wrapper.m_ControllerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @JumpAction.started += instance.OnJumpAction;
                @JumpAction.performed += instance.OnJumpAction;
                @JumpAction.canceled += instance.OnJumpAction;
                @ShootFlashlight.started += instance.OnShootFlashlight;
                @ShootFlashlight.performed += instance.OnShootFlashlight;
                @ShootFlashlight.canceled += instance.OnShootFlashlight;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @FlashlightAim.started += instance.OnFlashlightAim;
                @FlashlightAim.performed += instance.OnFlashlightAim;
                @FlashlightAim.canceled += instance.OnFlashlightAim;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @QuitGame.started += instance.OnQuitGame;
                @QuitGame.performed += instance.OnQuitGame;
                @QuitGame.canceled += instance.OnQuitGame;
            }
        }
    }
    public ControllerActions @Controller => new ControllerActions(this);
    public interface IControllerActions
    {
        void OnInteract(InputAction.CallbackContext context);
        void OnJumpAction(InputAction.CallbackContext context);
        void OnShootFlashlight(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnFlashlightAim(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnQuitGame(InputAction.CallbackContext context);
    }
}

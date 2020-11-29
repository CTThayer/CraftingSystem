using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _character;
    public PlayerCharacter character { get => _character; }
    
    [SerializeField] private Camera _characterCamera;
    public Camera characterCamera { get => _characterCamera; }

    [SerializeField] private GameObject _raycastOrigin;
    public GameObject raycastOrigin { get => _raycastOrigin; }

    [SerializeField] private PlayerHUDController HUD;
    [SerializeField] private float castDistance;
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private bool _isInteractionActive;
    public bool isInteractionActive 
    { 
        get => _isInteractionActive;
        set => _isInteractionActive = value;
    }

    // private variables
    private Interactable lastHitInteractable;
    private Interactable currentHitInteractable;
    private bool wasInteractingLastUpdate;
    private com.ootii.Cameras.CameraController camController;
    private int layerMask;

    void OnValidate()
    {
        if (_character == null)
            _character = this.GetComponent<PlayerCharacter>();
        Debug.Assert(_character != null);
        
        Debug.Assert(_characterCamera != null);
        camController = _characterCamera.transform.parent.GetComponent<com.ootii.Cameras.CameraController>();
        Debug.Assert(camController != null);

        Debug.Assert(_raycastOrigin != null);
        Debug.Assert(castDistance >= 0);
        layerMask = ~((1 << 2) | (1 << 8));     // Cast against everything except IgnoreRaycast and Player
        //Debug.Assert(layerMask >= 0);         // This is flagged b/c layerMask == -261 which might be correct, TODO: test this

        Debug.Assert(scrollSensitivity > 0);

        _isInteractionActive = true;
        wasInteractingLastUpdate = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        wasInteractingLastUpdate = false;
    }

    // Called at the same frequency as the physics system
    void FixedUpdate()
    {
        if (_isInteractionActive)
        {
            RaycastHit hitInfo;
            Ray ray = new Ray(_raycastOrigin.transform.position, _characterCamera.transform.forward);
            if (Physics.Raycast(ray, out hitInfo, castDistance, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                //currentHitInteractable = hitInfo.transform.gameObject.GetComponent<Interactable>();
                currentHitInteractable = hitInfo.transform.root.gameObject.GetComponent<Interactable>();
                if (currentHitInteractable != lastHitInteractable)
                    UpdateFocus();
                if (currentHitInteractable != null)
                    HandleInteraction();
            }
            else
            {
                currentHitInteractable = null;
                UpdateFocus();
                lastHitInteractable = null;
            }
        }
    }

    private void UpdateFocus()
    {
        if (lastHitInteractable != null)
            lastHitInteractable.OnHoverExit();
        if (currentHitInteractable != null)
            currentHitInteractable.OnHoverEnter();

        HUD.OnFocusChange(currentHitInteractable);

        lastHitInteractable = currentHitInteractable;
    }

    private void UpdateActionSelection(Interactable focus)
    {
        float delta = Input.mouseScrollDelta.y;
        if (delta != 0f)
        {
            int deltaVal = (int)(delta * scrollSensitivity);
            string s = focus.ChangeActionSelectionBy(deltaVal);
            HUD.OnActionSelectionChange(s);
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKey(KeyCode.E))
        {
            camController.IsZoomEnabled = false;
            UpdateActionSelection(currentHitInteractable);
            wasInteractingLastUpdate = true;
        }
        else if ( (wasInteractingLastUpdate && !Input.GetKey(KeyCode.E)) || Input.GetKeyUp(KeyCode.E) )
        {
            string actionResponse = currentHitInteractable.Interact(_character);
            HUD.OnActionResponse(actionResponse);
            camController.IsZoomEnabled = true;
            wasInteractingLastUpdate = false;
        }
    }

}

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

    private Vector3 reticlePosition;
    private Color debugRayColor = new Color(0f, 0f, 1f, 1f);

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

        reticlePosition = HUD.gameObject.transform.position;

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


            Vector3 rPosWorld = _characterCamera.ScreenToWorldPoint(reticlePosition);
            Vector3 direction = (raycastOrigin.transform.position - rPosWorld).normalized;

            //Ray ray = new Ray(_raycastOrigin.transform.position, _characterCamera.transform.forward);
            //Ray ray = _characterCamera.ScreenPointToRay(reticlePosition);
            Ray ray = new Ray(_raycastOrigin.transform.position, direction);

            Debug.DrawRay(_raycastOrigin.transform.position, direction * castDistance, debugRayColor);

            if (Physics.Raycast(ray, out hitInfo, castDistance, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                //currentHitInteractable = hitInfo.transform.gameObject.GetComponent<Interactable>();
                SetInFocusInteractable(hitInfo);
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

    public void SetInFocusInteractable(RaycastHit hitInfo)
    {
        Interactable i = hitInfo.transform.gameObject.GetComponent<Interactable>();
        if (i != null)
        {
            currentHitInteractable = i;
        }
        else
        {
            Interactable[] hitCluster = hitInfo.transform.root.gameObject.GetComponentsInChildren<Interactable>();
            if (hitCluster == null)
                return;
            else
            {
                foreach (Interactable interactable in hitCluster)
                {
                    if (interactable.gameObject == hitInfo.collider.gameObject)
                    {
                        currentHitInteractable = interactable;
                        return;
                    }
                }
                currentHitInteractable = null;
            }
        }
    }

}

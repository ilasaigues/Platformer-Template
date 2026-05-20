using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class SimpleMenu : MonoBehaviour
{

    [Header("References")]
    public List<Selectable> Selectables = new();
    [SerializeField] protected Selectable _firstSelected;

    [Header("Controls")]
    [SerializeField] protected InputActionReference _navigateReference;

    [Header("Sounds")]
    [SerializeField] protected UnityEvent SelectSoundEvent;

    protected Selectable _lastSelected;

    protected CanvasGroup CanvasGroup;

    protected SimpleMenu ParentMenu;

    [SerializeField]
    protected GameObject SpecialCloseMenu;


    public event Action OnMenuClosed = delegate { };




    void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        Selectables = GetComponentsInChildren<Selectable>().ToList();
        foreach (var selectable in Selectables)
        {
            AddListenersToSelectable(selectable);
        }
    }

    protected virtual IEnumerator SelectAfterDelay()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(_firstSelected != null ? _firstSelected.gameObject : null ?? Selectables[0].gameObject);
    }

    public virtual void OnEnable()
    {
        _navigateReference.action.performed += OnNavigate;
        StartCoroutine(SelectAfterDelay());
    }

    public virtual void OnDisable()
    {
        _navigateReference.action.performed -= OnNavigate;
    }

    protected virtual void AddListenersToSelectable(Selectable selectable)
    {
        //add event listener
        EventTrigger trigger = selectable.gameObject.GetOrAddComponent<EventTrigger>();

        //add SELECT event
        EventTrigger.Entry SelectEntry = new()
        {
            eventID = EventTriggerType.Select,
        };
        SelectEntry.callback.AddListener(OnSelectChild);
        trigger.triggers.Add(SelectEntry);


        //add DESELECT event
        EventTrigger.Entry DeselectEntry = new()
        {
            eventID = EventTriggerType.Deselect,
        };
        DeselectEntry.callback.AddListener(OnDeselectChild);
        trigger.triggers.Add(DeselectEntry);


        //add ONPOINTERENTER event
        EventTrigger.Entry PointerEnter = new()
        {
            eventID = EventTriggerType.PointerEnter,
        };
        PointerEnter.callback.AddListener(OnPointerEnterChild);
        trigger.triggers.Add(PointerEnter);

        //add ONPOINTEREXIT event
        EventTrigger.Entry PointerExit = new()
        {
            eventID = EventTriggerType.PointerExit,
        };
        PointerExit.callback.AddListener(OnPointerExitChild);
        trigger.triggers.Add(PointerExit);

    }

    public void OnSelectChild(BaseEventData eventData)
    {
        SelectSoundEvent?.Invoke();
        _lastSelected = eventData.selectedObject.GetComponent<Selectable>();
    }

    public void OnDeselectChild(BaseEventData eventData)
    {

    }


    public void OnPointerEnterChild(BaseEventData eventData)
    {
        if (eventData is PointerEventData pointerEventData)
        {
            Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if (sel == null)
            {
                sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }
            pointerEventData.selectedObject = sel.gameObject;
        }
    }
    public void OnPointerExitChild(BaseEventData eventData)
    {
        if (eventData is PointerEventData pointerEventData)
        {
            pointerEventData.selectedObject = null;
        }
    }

    protected virtual void OnNavigate(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
        }
    }

    public void OpenSubmenu(GameObject prefab)
    {
        var subMenu = Instantiate(prefab, transform.parent).GetComponent<SimpleMenu>();
        subMenu.OnMenuClosed += ChildMenuClosed;
        CanvasGroup.interactable = false;
    }

    private void ChildMenuClosed()
    {
        CanvasGroup.interactable = true;
    }

    public void CloseMenu()
    {
        if (!SpecialCloseMenu)
        {
            OnMenuClosed();
            Destroy(gameObject);
        }
        else
        {
            OpenSubmenu(SpecialCloseMenu);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}


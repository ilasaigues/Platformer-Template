using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Zenject;
using FMODUnity;
[RequireComponent(typeof(SoundEffectPlayer))]
[RequireComponent(typeof(CanvasGroup))]
public class SimpleMenu : MonoBehaviour
{
    UIInputHandler UIInputHandler;


    [Header("References")]
    public List<Selectable> Selectables = new();
    [SerializeField] protected Selectable _firstSelected;

    [Header("Controls")]
    [SerializeField] protected InputActionReference _navigateReference;


    protected Selectable _lastSelected;

    protected CanvasGroup CanvasGroup;

    protected SimpleMenu ParentMenu;

    [SerializeField]
    protected GameObject SpecialCloseMenu;

    public MenuSoundList MenuSounds;

    private SoundEffectPlayer _sfxPlayer;

    public event Action OnMenuClosed = delegate { };

    public BaseUITransition ShowTransition;
    public BaseUITransition HideTransition;
    private SimpleMenu _subMenu;

    void Awake()
    {
        _sfxPlayer = GetComponent<SoundEffectPlayer>();
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

    public void SetInputHandler(UIInputHandler inputHandler)
    {
        UIInputHandler = inputHandler;
        UIInputHandler.OnMenuButtonPressed += MenuButtonPressed;
    }

    void OnDestroy()
    {
        UIInputHandler.OnMenuButtonPressed -= MenuButtonPressed;
    }

    void MenuButtonPressed()
    {
        CloseMenu();
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

    public void PlayConfirmSound()
    {
        _sfxPlayer.PlaySFX(MenuSounds.ConfirmSoundEventReference.eventReference);
    }

    public void PlayCancelSound()
    {
        _sfxPlayer.PlaySFX(MenuSounds.CancelMenuSoundEventReference.eventReference);
    }

    public void PlaySFXAdjustSound()
    {
        _sfxPlayer.PlaySFX(MenuSounds.SFXVolumeChangeSoundEventReference.eventReference);
    }

    public void PlayBGMAdjustSound()
    {
        _sfxPlayer.PlaySFX(MenuSounds.BGMVolumeChangeSoundEventReference.eventReference);
    }

    public void OnSelectChild(BaseEventData eventData)
    {
        _sfxPlayer.PlaySFX(MenuSounds.HoverSoundEventReference.eventReference);
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

    public async void OpenSubmenu(GameObject prefab)
    {
        PlayConfirmSound();
        CanvasGroup.interactable = false;
        _subMenu = Instantiate(prefab, transform.parent).GetComponent<SimpleMenu>();
        if (UIInputHandler != null)
        {
            _subMenu.SetInputHandler(UIInputHandler);
        }
        await _subMenu.ShowMenu();
        _subMenu.OnMenuClosed += ChildMenuClosed;
    }


    private async void ChildMenuClosed()
    {
        await ShowMenu(false);
        CanvasGroup.interactable = true;
    }

    async Task ShowMenu(bool doTransition = true)
    {
        if (doTransition) await ShowTransition?.DoTransition(gameObject);
    }

    async Task HideMenu()
    {
        await HideTransition?.DoTransition(gameObject);
    }

    public async void CloseMenu()
    {
        if (_subMenu != null)
        {
            _subMenu.CloseMenu();
            return;
        }

        if (!SpecialCloseMenu)
        {
            CanvasGroup.interactable = false;
            OnMenuClosed();
            PlayCancelSound();
            await HideMenu();
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


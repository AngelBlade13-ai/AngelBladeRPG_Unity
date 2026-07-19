using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Keeps a keyboard/gamepad selection valid across menus whose buttons are
/// shown, hidden, or disabled at runtime instead of being replaced. Unity's
/// EventSystem clears its selection whenever the selected object becomes
/// inactive or non-interactable and never re-selects on its own. Also marks
/// the selected button's label with a non-color-only indicator, since the
/// default color-tint highlight is easy to miss and this project already
/// avoids color-only state elsewhere.
/// </summary>
public static class UIFocusHelper
{
    private const string FocusMarker = "* ";

    private static TextMeshProUGUI markedLabel;

    public static void Select(GameObject target)
    {
        if (EventSystem.current == null || !IsUsable(target))
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(target);
        RefreshSelectionMarker();
    }

    public static void Select(Selectable target)
    {
        Select(target == null ? null : target.gameObject);
    }

    public static void SelectFirstAvailable(params GameObject[] candidates)
    {
        if (candidates == null || EventSystem.current == null)
        {
            return;
        }

        foreach (GameObject candidate in candidates)
        {
            if (IsUsable(candidate))
            {
                EventSystem.current.SetSelectedGameObject(candidate);
                RefreshSelectionMarker();
                return;
            }
        }
    }

    public static void SelectFirstAvailable(params Selectable[] candidates)
    {
        if (candidates == null || EventSystem.current == null)
        {
            return;
        }

        foreach (Selectable candidate in candidates)
        {
            if (candidate != null && IsUsable(candidate.gameObject))
            {
                EventSystem.current.SetSelectedGameObject(candidate.gameObject);
                RefreshSelectionMarker();
                return;
            }
        }
    }

    public static bool CurrentSelectionIsUsable()
    {
        return EventSystem.current != null &&
            IsUsable(EventSystem.current.currentSelectedGameObject);
    }

    public static void Clear()
    {
        EventSystem.current?.SetSelectedGameObject(null);
        RefreshSelectionMarker();
    }

    /// <summary>
    /// Re-applies the focus marker to whichever label the EventSystem
    /// currently has selected, and strips it from the previous one. Safe to
    /// call every frame; it repairs labels that other code overwrites (for
    /// example a battle command button whose text changes while selected).
    /// </summary>
    public static void RefreshSelectionMarker()
    {
        GameObject current = EventSystem.current != null
            ? EventSystem.current.currentSelectedGameObject
            : null;
        TextMeshProUGUI currentLabel = current != null && current.activeInHierarchy
            ? current.GetComponentInChildren<TextMeshProUGUI>()
            : null;

        if (currentLabel != markedLabel)
        {
            RemoveMarker(markedLabel);
            markedLabel = currentLabel;
        }

        AddMarker(markedLabel);
    }

    private static void AddMarker(TextMeshProUGUI label)
    {
        if (label != null && !label.text.StartsWith(FocusMarker))
        {
            label.text = FocusMarker + label.text;
        }
    }

    private static void RemoveMarker(TextMeshProUGUI label)
    {
        if (label != null && label.text.StartsWith(FocusMarker))
        {
            label.text = label.text.Substring(FocusMarker.Length);
        }
    }

    private static bool IsUsable(GameObject target)
    {
        if (target == null || !target.activeInHierarchy)
        {
            return false;
        }

        return !target.TryGetComponent(out Selectable selectable) ||
            selectable.IsInteractable();
    }
}

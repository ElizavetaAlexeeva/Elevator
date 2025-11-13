using UnityEngine;

public interface IInteractable 
{
    void OnSelect();      // Вызывается, когда предмет наведён
    void OnDeselect();    // Вызывается, когда курсор ушёл
    void OnInteract();    // Вызывается при нажатии E
}

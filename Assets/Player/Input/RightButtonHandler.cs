using UnityEngine;
using UnityEngine.EventSystems;

public class RightButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private PlayerInputState _inputState;

    public void OnPointerDown(PointerEventData eventData)
    {
        _inputState.SetMove(Vector2.right);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _inputState.SetMove(Vector2.zero);
    }
}

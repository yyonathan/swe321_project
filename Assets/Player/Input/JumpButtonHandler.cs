using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private PlayerInputState _inputState;

    public void Awake()
    {
        _inputState.SetSupportsJumpCut(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _inputState.SetJumpPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _inputState.SetJumpReleased(true);
    }

}









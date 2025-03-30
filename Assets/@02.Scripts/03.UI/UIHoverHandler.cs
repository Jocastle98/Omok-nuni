using AudioEnums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UIHoverHandler : MonoBehaviour, IPointerEnterHandler
{
    public Animator Animator;
    
    private static readonly int IsMouseOverHash = Animator.StringToHash("isMouseOver");

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Animator != null)
        {
            Animator.SetTrigger(IsMouseOverHash);
            AudioManager.Instance.PlayAudioClip(ESfxType.BranchRustle);
        }
    }
}
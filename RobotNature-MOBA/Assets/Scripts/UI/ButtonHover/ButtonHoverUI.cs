using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform Champion1Button ;

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        Champion1Button.GetComponent<Animator>().Play("ON");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Champion1Button.GetComponent<Animator>().Play("OFF");
    }
}

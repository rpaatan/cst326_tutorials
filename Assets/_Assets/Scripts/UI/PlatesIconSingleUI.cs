using UnityEngine;
using UnityEngine.UI; 


public class PlatesIconSingleUI : MonoBehaviour
{
    [SerializeField] private Image image; 
    
    public void setKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        image.sprite = kitchenObjectSO.sprite; 
    }
}

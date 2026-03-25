using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;  
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                // player if carrying something 
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();

                    }
                }
                else
                {
                   // player isn't carring a plate, but something else 
                   if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                   {
                       // counter is holding a plate
                       plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO());
                       {
                           player.GetKitchenObject().DestroySelf();
                       }
                   }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}

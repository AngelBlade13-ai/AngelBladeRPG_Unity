using UnityEngine;

public sealed class ShopInteractable2D : MonoBehaviour, IWorldInteractable
{
    [SerializeField] private string shopId;
    [SerializeField] private ShopPanel panel;

    public bool CanInteract(GameObject interactor)
    {
        return panel != null && ShopCatalog.Get(shopId) != null &&
            GameSessionStore.Current.HasPlayer;
    }

    public void Interact(GameObject interactor)
    {
        panel.Open(interactor, shopId);
    }

    public void Configure(string serviceShopId, ShopPanel servicePanel)
    {
        shopId = serviceShopId;
        panel = servicePanel;
    }
}

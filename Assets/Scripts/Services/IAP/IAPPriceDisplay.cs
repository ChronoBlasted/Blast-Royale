using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

public class IAPPriceDisplay : MonoBehaviour, IDetailedStoreListener
{
    public string _productId = "your_iap_id_here";
    public TMP_Text _priceTxt;

    IStoreController _storeController;

    public void Init()
    {
        if (_storeController == null)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(_productId, ProductType.Consumable);
            UnityPurchasing.Initialize(this, builder);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
        Product product = _storeController.products.WithID(_productId);

        if (product != null && product.availableToPurchase)
        {
            string currencyCode = product.metadata.isoCurrencyCode;
            decimal price = product.metadata.localizedPrice;

            var culture = GetCultureFromCurrencyCode(currencyCode);
            string currencySymbol = new RegionInfo(culture.LCID).CurrencySymbol;

            _priceTxt.text = currencySymbol + price.ToString("F2");
        }
        else
        {
            _priceTxt.text = "Not available";
        }
    }

    private CultureInfo GetCultureFromCurrencyCode(string currencyCode)
    {
        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            var region = new RegionInfo(culture.LCID);
            if (region.ISOCurrencySymbol == currencyCode)
            {
                return culture;
            }
        }

        return CultureInfo.InvariantCulture;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Init failed: " + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) { }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) => PurchaseProcessingResult.Complete;

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Init failed: {error}, Message: {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
    }
}

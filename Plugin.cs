using BepInEx;
using HarmonyLib;

namespace CashOnly
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded! Applying patch...");
            Harmony harmony = new Harmony("com.orpticon.CashOnly");
            harmony.PatchAll();
        }
    }
    public static class PaymentFixerPatch
    {
        [HarmonyPatch(typeof(Customer), "DoPayment")]
        public static class Customer_DoPayment_Patch
        {
            public static void Prefix(ref bool viaCreditCard)
            {
                viaCreditCard = false;
            }
        }
        [HarmonyPatch(typeof(Checkout), "ChangeState")]
        public static class Checkout_ChangeState_Patch
        {
            public static void Prefix(ref Checkout.State newState)
            {
                if (newState == Checkout.State.CUSTOMER_HANDING_CARD) newState = Checkout.State.CUSTOMER_HANDING_CASH;
                if (newState == Checkout.State.PAYMENT_CREDIT_CARD) newState = Checkout.State.PAYMENT_CASH;
            }
        }
    }
}

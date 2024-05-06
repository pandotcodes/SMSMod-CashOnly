using BepInEx;
using HarmonyLib;
using MyBox;

namespace CashOnly
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInIncompatibility("CardOnly")]
    public class Plugin : BaseUnityPlugin
    {
        bool patched = false;
        Harmony harmony;
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            harmony = new Harmony("com.orpticon.CashOnly");
        }
        private void Update()
        {
            if (Singleton<OnboardingManager>.Instance != null && Singleton<OnboardingManager>.Instance.Completed && !patched)
            {
                patched = true;
                Logger.LogInfo("Patching...");
                // Plugin startup logic
                harmony.PatchAll();
            }
            if (Singleton<OnboardingManager>.Instance != null && !Singleton<OnboardingManager>.Instance.Completed && patched)
            {
                patched = false;
                Logger.LogInfo("Unpatching...");
                harmony.UnpatchSelf();
            }
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

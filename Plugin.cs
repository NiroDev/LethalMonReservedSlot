using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ReservedItemSlotCore;
using ReservedItemSlotCore.Data;
using UnityEngine;

namespace LethalMonReservedSlot
{
    [BepInDependency(ReservedItemSlotCoreName, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(LethalMonName, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string ReservedItemSlotCoreName = "FlipMods.ReservedItemSlotCore";
        public const string LethalMonName = "Feiryn.LethalMon";

        public const string modGUID = $"Niro.{modName}";
        public const string modName = "LethalMonReservedSlot";
        public const string modVersion = "1.0.0";

        private static readonly Vector3 BallRotationOffset = new Vector3(90f, 0f, 0f);
        private static readonly Vector3 BallPositionOffset = new Vector3(0.2f, 0.25f, 0f);

        private readonly Harmony harmony = new(modGUID);

        private static Plugin instance;

        static internal ManualLogSource mls;

        public static ReservedItemSlotData pokeballSlotData = null;
        public static ReservedItemData pokeballData;
        public static ReservedItemData greatballData;
        public static ReservedItemData ultraballData;
        public static ReservedItemData masterballData;

        public static ConfigFile BepInExConfig() { return instance.Config; }

        public void Awake()
        {
            instance ??= this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            LethalMonReservedSlot.Config.Instance.Setup();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ReservedItemSlotCoreName))
                CreateReservedItemSlots();

            mls.LogMessage("Plugin " + modName + " loaded!");
        }

        void CreateReservedItemSlots()
        {
#if DEBUG
            LethalMonReservedSlot.Config.Instance.UnlockPrice.Value = 0;
#endif

            pokeballSlotData = ReservedItemSlotData.CreateReservedItemSlotData("PokeballSlot", LethalMonReservedSlot.Config.Instance.SlotPriority.Value, LethalMonReservedSlot.Config.Instance.UnlockPrice.Value);

            if (LethalMonReservedSlot.Config.Instance.UnlockPrice.Value == 0)
                UnlockItemSlot();

            pokeballData = pokeballSlotData.AddItemToReservedItemSlot(new ReservedItemData("Pokeball", LethalMonReservedSlot.Config.Instance.AttachTo.Value, BallPositionOffset, BallRotationOffset));
            greatballData = pokeballSlotData.AddItemToReservedItemSlot(new ReservedItemData("Great Ball", LethalMonReservedSlot.Config.Instance.AttachTo.Value, BallPositionOffset, BallRotationOffset));
            ultraballData = pokeballSlotData.AddItemToReservedItemSlot(new ReservedItemData("Ultra Ball", LethalMonReservedSlot.Config.Instance.AttachTo.Value, BallPositionOffset, BallRotationOffset));
            masterballData = pokeballSlotData.AddItemToReservedItemSlot(new ReservedItemData("Master Ball", LethalMonReservedSlot.Config.Instance.AttachTo.Value, BallPositionOffset, BallRotationOffset));
        }

        internal static void AttachToChanged()
        {
            if (pokeballSlotData == null) return;

            pokeballData.holsteredParentBone = LethalMonReservedSlot.Config.Instance.AttachTo.Value;
            greatballData.holsteredParentBone = LethalMonReservedSlot.Config.Instance.AttachTo.Value;
            ultraballData.holsteredParentBone = LethalMonReservedSlot.Config.Instance.AttachTo.Value;
            masterballData.holsteredParentBone = LethalMonReservedSlot.Config.Instance.AttachTo.Value;
        }

        internal static void SlotPriorityChanged()
        {
            if (pokeballSlotData == null) return;
            pokeballSlotData.slotPriority = LethalMonReservedSlot.Config.Instance.SlotPriority.Value;
        }

        internal static void UnlockPriceChanged()
        {
            if (pokeballSlotData == null) return;
            var price = LethalMonReservedSlot.Config.Instance.UnlockPrice.Value;
            if (price <= 0)
                UnlockItemSlot();
            else
                pokeballSlotData.purchasePrice = price;
        }

        internal static void UnlockItemSlot() => SessionManager.UnlockReservedItemSlot(pokeballSlotData);
    }
}

using BepInEx.Configuration;
using ReservedItemSlotCore.Data;

namespace LethalMonReservedSlot
{
    public sealed class Config
    {
        #region Properties
        public ConfigEntry<int> UnlockPrice { get; set; }
        public ConfigEntry<int> SlotPriority { get; set; }
        public ConfigEntry<PlayerBone> AttachTo { get; set; }

        private static Config instance = null;
        public static Config Instance
        {
            get
            {
                instance ??= new Config();
                return instance;
            }
        }
        #endregion

        public void Setup()
        {
            UnlockPrice  = Plugin.BepInExConfig().Bind("General", "UnlockPrice", 50, new ConfigDescription("Cost of unlocking this slot (0 = unlocked at start)."));
            SlotPriority = Plugin.BepInExConfig().Bind("General", "SlotPriority", 20, new ConfigDescription("Priority for this item slot relative to other item slots.", new AcceptableValueRange<int>(0, 100)));
            AttachTo = Plugin.BepInExConfig().Bind("General", "AttachTo", PlayerBone.RightShoulder, new ConfigDescription("Defines where the player holds the ball from other players point of view. (Restart required for now)"));

            UnlockPrice.SettingChanged += (obj, args) => { Plugin.UnlockPriceChanged(); };
            SlotPriority.SettingChanged += (obj, args) => { Plugin.SlotPriorityChanged(); };
            AttachTo.SettingChanged += (obj, args) => { Plugin.AttachToChanged(); };
        }
    }
}


using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using System;
using UnityEngine;

namespace DaggerfallTheme
{
    public class NoEquipDelay : MonoBehaviour
    {
        private static Mod mod;

        private float m_EquipDelay = 0f;

        private bool m_NeedsOverride = false;

        //Methods
        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<NoEquipDelay>();
        }

        private void Awake()
        {
            ModSettings settings = mod.GetSettings();
            if (settings != null)
            {
                m_EquipDelay = settings.GetValue<float>("Settings", "EquipDelay");
            }
            DaggerfallUI.UIManager.OnWindowChange += OnWindowChange;
            mod.IsReady = true;
        }

        private void OnWindowChange(object sender, EventArgs args)
        {
            IUserInterfaceWindow currentWindow = DaggerfallUI.UIManager.TopWindow;
            if (currentWindow == DaggerfallUI.Instance.InventoryWindow)
            {
                m_NeedsOverride = true;
            }
            else if (currentWindow == DaggerfallUI.Instance.DaggerfallHUD && m_NeedsOverride)
            {
                if (GameManager.Instance.WeaponManager.EquipCountdownRightHand > m_EquipDelay ||
                    GameManager.Instance.WeaponManager.EquipCountdownLeftHand > m_EquipDelay)
                {
                    GameManager.Instance.WeaponManager.EquipCountdownRightHand = m_EquipDelay * 1000;
                    GameManager.Instance.WeaponManager.EquipCountdownLeftHand = m_EquipDelay * 1000;
                }
                m_NeedsOverride = false;
            }
        }
    }
}
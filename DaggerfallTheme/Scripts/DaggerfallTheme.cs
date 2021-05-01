using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using System;
using System.Collections;
using UnityEngine;

namespace DaggerfallTheme
{
    public class DaggerfallTheme : MonoBehaviour
    {
        private static Mod mod;

        private AudioSource m_AudioSource;
        private bool m_MainWIndowMet = false;
        private bool m_ThemeHasPlayed = false;

        //Sound Settings
        private string[] m_ThemeVersions =
        {
            "Pieces of 8-bit (Classic HQ)",
            "Ramon Molesworth",
            "Rich Douglas"
        };
        private float m_BaseVolume;
        private const float m_FadeOutDuration = 3f;

        //Methods
        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<AudioSource>();
            go.AddComponent<DaggerfallTheme>();
        }

        private void Awake()
        {
            if (InitAudioSource())
            {
                DaggerfallUI.UIManager.OnWindowChange += OnWindowChange;
                mod.IsReady = true;
            }
        }

        private bool InitAudioSource()
        {
            m_AudioSource = GetComponent<AudioSource>();
            ModSettings settings = mod.GetSettings();
            if (m_AudioSource != null)
            {
                if (settings != null)
                {
                    m_BaseVolume = settings.GetValue<float>("Settings", "Volume");
                    m_AudioSource.clip = mod.GetAsset<AudioClip>(m_ThemeVersions[settings.GetValue<int>("Settings", "Version")]);
                    m_AudioSource.playOnAwake = false;
                    m_AudioSource.loop = settings.GetValue<bool>("Settings", "Loop"); ;
                }
                else
                {
                    Debug.LogError("DaggerfallTheme: Unable to retrieve mod settings.");
                }
            }
            else
            {
                Debug.LogError("DaggerfallTheme: Could not initialize audiosource properly, theme will not play.");
            }
            return m_AudioSource != null && settings != null;
        }

        private void PlayMainTheme()
        {
            m_ThemeHasPlayed = true;
            if (m_BaseVolume > 0)
            {
                m_AudioSource.volume = m_BaseVolume;
                m_AudioSource.Play();
            }
        }

        private void StopMainTheme(bool fade)
        {
            DaggerfallUI.UIManager.OnWindowChange -= OnWindowChange;
            StartCoroutine(FadeOutMainTheme(fade));
        }

        private IEnumerator FadeOutMainTheme(bool fade)
        {
            if (fade)
            {
                float volumeStep = m_AudioSource.volume / m_FadeOutDuration;
                while (m_AudioSource.volume > 0)
                {
                    m_AudioSource.volume -= volumeStep * Time.deltaTime;
                    yield return null;
                }
            }
            Destroy(gameObject);
        }

        private void OnWindowChange(object sender, EventArgs args)
        {
            IUserInterfaceWindow currentWindow = DaggerfallUI.UIManager.TopWindow;

            if (currentWindow.GetType() == typeof(DaggerfallStartWindow) && !m_ThemeHasPlayed)
            {
                if (m_MainWIndowMet)
                {
                    PlayMainTheme();
                }
                m_MainWIndowMet = true;
            }
            else if (currentWindow.GetType() == typeof(DaggerfallHUD) && m_ThemeHasPlayed)
            {
                StopMainTheme(true);
            }
            else if (currentWindow.GetType() == typeof(DaggerfallVidPlayerWindow) && m_ThemeHasPlayed)
            {
                StopMainTheme(false);
            }
        }
    }
}
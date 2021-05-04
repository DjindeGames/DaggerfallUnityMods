using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallTheme
{
    public class QuestMarkers : MonoBehaviour
    {
        private static Mod mod;

        private Canvas m_Canvas;
        private QuestMarkerComponent m_QuestMarker;

        private Vector3 m_CurrentQuestMarkerPosition = Vector3.zero;

        //Methods
        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<QuestMarkers>();
        }

        private void Awake()
        {
            GameObject canvas = Instantiate(mod.GetAsset<GameObject>("QuestMarkersCanvas"), transform);
            GameObject marker = Instantiate(mod.GetAsset<GameObject>("QuestMarker"), canvas.transform);
            m_QuestMarker = marker.GetComponent<QuestMarkerComponent>();
            ModSettings settings = mod.GetSettings();
            if (settings != null)
            {
                m_QuestMarker.SetIconSize(settings.GetValue<float>("Settings", "IconSize"));
                m_QuestMarker.SetFontSize(settings.GetValue<int>("Settings", "FontSize"));
                m_QuestMarker.SetColor(settings.GetValue<Color32>("Settings", "MarkerColor"));
            }
            mod.IsReady = true;
        }

        private void Update()
        {
            QuestMarker spawnMarker;
            Vector3 buildingOrigin;

            if (QuestMachine.Instance.GetCurrentLocationQuestMarker(out spawnMarker, out buildingOrigin))
            {
                Quest quest = GameManager.Instance.QuestMachine.GetQuest(spawnMarker.questUID);
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                {
                    m_QuestMarker.Display();
                    Vector3 dungeonBlockPosition = new Vector3(spawnMarker.dungeonX * RDBLayout.RDBSide, 0, spawnMarker.dungeonZ * RDBLayout.RDBSide);
                    m_CurrentQuestMarkerPosition = dungeonBlockPosition + spawnMarker.flatPosition;
                }
                else if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
                {
                    m_QuestMarker.Display();
                    GameManager.Instance.PlayerEnterExit.Interior.FindClosestMarker(out m_CurrentQuestMarkerPosition,
                        (DaggerfallInterior.InteriorMarkerTypes)spawnMarker.markerType,
                        GameManager.Instance.PlayerObject.transform.position);
                }
                else
                {
                    m_QuestMarker.Hide();
                }
            }
            else
            {
                m_QuestMarker.Hide();
            }
            m_QuestMarker.SetDistance((int)Vector3.Distance(GameManager.Instance.PlayerObject.transform.position, m_CurrentQuestMarkerPosition));
            m_QuestMarker.SetWorldPosition(m_CurrentQuestMarkerPosition);
        }
    }
}
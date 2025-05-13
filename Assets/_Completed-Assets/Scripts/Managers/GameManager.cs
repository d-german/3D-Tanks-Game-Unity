using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 5;
        public float m_StartDelay = 3f;
        public float m_EndDelay = 3f;
        public CameraControl m_CameraControl;

        public Text m_MessageTextComponent;
        public GameObject m_TankPrefab;

        public TankManager[] m_Tanks;

        private int m_RoundNumber;
        private WaitForSeconds m_StartWait;
        private WaitForSeconds m_EndWait;

        private TankManager m_RoundWinner;

        private TankManager m_GameWinner;

        private GameMessageUIService m_GameMessageUIService;
        private GameRulesManager m_GameRulesManager;

        private void Start()
        {
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            m_GameMessageUIService = new GameMessageUIService(m_MessageTextComponent);
            m_GameRulesManager = new GameRulesManager(m_NumRoundsToWin);

            SpawnAllTanks();
            SetCameraTargets();

            StartCoroutine(GameLoop());
        }

        private void SpawnAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].m_Instance =
                    Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position,
                        m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
        }

        private void SetCameraTargets()
        {
            Transform[] targets = new Transform[m_Tanks.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = m_Tanks[i].m_Instance.transform;
            }

            m_CameraControl.m_Targets = targets;
        }

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting());

            yield return StartCoroutine(RoundPlaying());

            yield return StartCoroutine(RoundEnding());

            if (m_GameWinner != null)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                StartCoroutine(GameLoop());
            }
        }

        private IEnumerator RoundStarting()
        {
            ResetAllTanks();
            DisableTankControl();

            m_CameraControl.SetStartPositionAndSize();

            m_RoundNumber++;
            m_GameMessageUIService.DisplayRoundStart(m_RoundNumber);

            yield return m_StartWait;
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl();

            m_GameMessageUIService.ClearMessage();

            while (!m_GameRulesManager.IsOnlyOneTankLeft(m_Tanks))
            {
                yield return null;
            }
        }

        private IEnumerator RoundEnding()
        {
            DisableTankControl();
            m_RoundWinner = null;

            m_RoundWinner = m_GameRulesManager.DetermineRoundWinner(m_Tanks);

            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            m_GameWinner = m_GameRulesManager.DetermineGameWinner(m_Tanks);

            m_GameMessageUIService.DisplayRoundEndResults(m_RoundWinner, m_GameWinner, m_Tanks);

            yield return m_EndWait;
        }

        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            }
        }

        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }

        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }
    }
}
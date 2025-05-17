using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Rules Configuration")] public int m_NumRoundsToWin = 3;
        public float m_RoundDurationSeconds = 60f;

        public enum GameMode
        {
            ClassicDeathmatch,
            TimedRounds
        }

        public GameMode m_SelectedGameMode = GameMode.TimedRounds;

        [Header("Game Delays")] public float m_StartDelay = 3f;
        public float m_EndDelay = 3f;

        [Header("References")] public CameraControl m_CameraControl;
        public Text m_MessageTextComponent;
        public GameObject m_TankPrefab;
        public TankManager[] m_Tanks;

        private int m_RoundNumber;
        private WaitForSeconds m_StartWait;
        private WaitForSeconds m_EndWait;
        private TankManager m_RoundWinner;
        private TankManager m_GameWinner;

        private IGameMessageUIService m_GameMessageUIService;
        private IGameRulesStrategy m_GameRulesStrategy;

        private void Start()
        {
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            ILogger uiServiceLogger = new UnityLogger();
            IGameMessageUIService concreteMessageService = new GameMessageUIService(m_MessageTextComponent);
            m_GameMessageUIService = new LoggingGameMessageUIServiceDecorator(concreteMessageService, uiServiceLogger);

            switch (m_SelectedGameMode)
            {
                case GameMode.TimedRounds:
                    m_GameRulesStrategy = new TimedRoundRulesStrategy(m_RoundDurationSeconds, m_NumRoundsToWin, new UnityTimeProvider());
                    Debug.Log(
                        $"Game Mode: Timed Rounds ({m_RoundDurationSeconds}s per round, {m_NumRoundsToWin} to win game)");
                    break;
                case GameMode.ClassicDeathmatch:
                default:
                    m_GameRulesStrategy = new ClassicDeathmatchRulesStrategy(m_NumRoundsToWin);
                    Debug.Log($"Game Mode: Classic Deathmatch ({m_NumRoundsToWin} to win game)");
                    break;
            }

            if (m_Tanks == null || m_Tanks.Length == 0)
            {
                Debug.LogError("GameManager: m_Tanks array is not assigned or empty in the Inspector!");
                return;
            }

            if (m_TankPrefab == null) Debug.LogError("GameManager: m_TankPrefab is not assigned!");
            if (m_MessageTextComponent == null) Debug.LogError("GameManager: m_MessageTextComponent is not assigned!");
            if (m_CameraControl == null) Debug.LogError("GameManager: m_CameraControl is not assigned!");

            SpawnAllTanks();
            SetCameraTargets();

            StartCoroutine(GameLoop());
        }

        private void SpawnAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i] == null || m_Tanks[i].m_SpawnPoint == null)
                {
                    Debug.LogError($"GameManager: Tank config or spawn point for tank index {i} is null.");
                    continue;
                }

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
            int validTargets = 0;
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i] != null && m_Tanks[i].m_Instance != null)
                {
                    targets[validTargets++] = m_Tanks[i].m_Instance.transform;
                }
            }

            if (validTargets < targets.Length) System.Array.Resize(ref targets, validTargets);

            if (m_CameraControl != null) m_CameraControl.m_Targets = targets;
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

            if (m_CameraControl != null) m_CameraControl.SetStartPositionAndSize();

            m_RoundNumber++;
            m_GameRulesStrategy.StartRound();
            m_GameMessageUIService.DisplayRoundStart(m_RoundNumber);

            yield return m_StartWait;
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl();
            m_GameMessageUIService.ClearMessage();

            while (!m_GameRulesStrategy.IsRoundOver(m_Tanks))
            {
                yield return null;
            }
        }

        private IEnumerator RoundEnding()
        {
            DisableTankControl();
            m_RoundWinner = null;

            m_RoundWinner = m_GameRulesStrategy.DetermineRoundWinner(m_Tanks);

            if (m_RoundWinner != null)
            {
                m_RoundWinner.m_Wins++;
            }

            m_GameWinner = m_GameRulesStrategy.DetermineGameWinner(m_Tanks);

            m_GameMessageUIService.DisplayRoundEndResults(m_RoundWinner, m_GameWinner, m_Tanks);

            yield return m_EndWait;
        }

        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i] != null) m_Tanks[i].Reset();
            }
        }

        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i] != null) m_Tanks[i].EnableControl();
            }
        }

        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i] != null) m_Tanks[i].DisableControl();
            }
        }
    }
}
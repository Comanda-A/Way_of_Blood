using UnityEngine;
using UnityEngine.SceneManagement;
using WayOfBlood.Character.Player;

namespace WayOfBlood.GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] GameObject DefeatScreen;

        private PlayerController playerController;

        private void Start()
        {
            Application.targetFrameRate = 60;

            var player = GetPlayer();
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
                playerController.OnDeath += OnDeathPlayer;
            }
            else
            {
                Debug.LogError("PlayerController not found!");
            }
        }

        // метод для получения PlayerController
        public static GameObject GetPlayer()
        {
            // Находим все объекты с тегом "Player" (включая неактивные)
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject playerObj in playerObjects)
            {
                // Проверяем, есть ли компонент PlayerController у объекта
                PlayerController controller = playerObj.GetComponent<PlayerController>();
                if (controller != null)
                {
                    return playerObj;
                }
            }

            return null;
        }

        private void OnDeathPlayer()
        {
            DefeatScreen.SetActive(true);
        }

        public void RestartGame()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        private void OnDestroy()
        {
            if (playerController != null)
            {
                playerController.OnDeath -= OnDeathPlayer;
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
using RQ.Base.Skill;
using RQ.Base.VariableClasses;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Controller;
using RQ.Controller.Inventory;
using RQ.Controller.Player;
using RQ.Controller.Scene;
using UnityEngine;

namespace RQ.Base.Manager
{
    // We are keeping this a MonoBehavior because it not only keeps this alive, but it keeps alive
    // everything within this gameObject, such as the AudioSource object so the sounds and music don't stop or skip
    // between scene loads
    /// <summary>
    /// Controls the state of the game as the player progresses through it.
    /// </summary>
    [AddComponentMenu("RQ/Manager/Game State")]
    public class GameStateController : ComponentBase<GameStateController>
    {
        //public SceneConfig NextSceneConfig { get; set; }
        public string SpawnpointUniqueId { get; set; }
        public SceneConfig CurrentSceneConfig { get; set; }
        public MoldConfig CurrentMold { get; set; }
        public ShardConfig CurrentShard { get; set; }
        [SerializeField]
        private Variables _globalVariables;
        public Variables GlobalVariables => _globalVariables;
        // TODO Set this to true when the player starts a new game!
        public bool BeginNewGame { get; set; }
        public bool ChangingScene { get; set; }
        [SerializeField]
        private SceneController _sceneController;

        protected override void Awake()
        {
            base.Awake();
            //BeginNewGame = true;
        }

        public override string GetId()
        {
            return "Game State Controller";
        }

        public override void Init()
        {
            if (_hasInited)
                return;
            base.Init();

            GlobalVariables.Init();
        }

        /// <summary>
        /// Singleton
        /// </summary>
        private static GameStateController _instance;
        [HideInInspector]
        public static GameStateController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameStateController>();
                    _instance.Init();
                }
                return _instance;
            }
        }

        public void ToggleMold(bool isDown)
        {
            var player = EntityContainer.Instance.GetMainCharacter();
            //var playerController = player.Components.GetComponent<PlayerController>();
            var inventoryController = player.Components.GetComponent<InventoryComponent>();
            //inventoryController.
        }

        public void LoadScene(string sceneName, string spawnPointId)
        {
            ChangingScene = true;
            // Log the next Spawnpoint before ClearScene deletes it
            SpawnpointUniqueId = spawnPointId;
            //if (GameDataController.Instance.Data != null)
            //{
            //    GameDataController.Instance.Data.SpawnpointUniqueId =
            //        GameDataController.Instance.Data.NextSpawnpointUniqueId;
            //}
            //ClearScene(false);

            //MessageDispatcher.Instance.RemoveByEarlyTermination(Enums.TelegramEarlyTermination.ChangeScenes);
            //Debug.Log("LoadLevel being called");
            //Debug.Log("Loading scene, entity count = " + EntityContainer._instance.EntityInstanceMap.Count);
            //Application.LoadLevel(sceneName);

            _sceneController.FadeAndLoadScene(sceneName);
            //Debug.Log("(GameStateController) FadeAndLoadScene called");


        }
    }
}

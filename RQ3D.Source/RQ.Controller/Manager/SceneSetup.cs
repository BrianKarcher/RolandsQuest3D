using RQ.Base.Item;
using RQ.Base.Manager;
using RQ.Base.Skill;
using RQ.Base.SpawnPoint;
using RQ.Common.Container;
using RQ.Controller.Scene;
using RQ.Messaging;
using System;
using System.Linq;
using UnityEngine;

namespace RQ.Controller.Manager
{
    [AddComponentMenu("RQ/Common/Scene Setup")]
    public class SceneSetup : MonoBehaviour
    {
        public SceneConfig SceneConfig;
        [SerializeField]
        private GameObject SpawnPoints;

        [SerializeField]
        private GameObject _actorsRoot;

        private SpawnPointComponent[] _spawnPointComponents;

        private IEntity _player;

        private void Awake()
        {
            InitAllEntities();
        }

        private void Start()
        {
            _player = EntityContainer.Instance.GetMainCharacter();
            if (GameStateController.Instance.BeginNewGame)
            {
                BeginNewGame();
                // Set this variable in a FSM state... maybe the beginning of Play mode?
                GameStateController.Instance.BeginNewGame = false;
            }
            PlacePlayerAtSpawnPoint();
        }

        public void InitAllEntities()
        {
            //Debug.Log(this.name + " - InitAllEntities called");
            //var actorsRoot = GameController.Instance.GetSceneSetup().GetActorsRoot();
            var entities = _actorsRoot.GetComponentsInChildren<ComponentRepository>(true);
            foreach (var entity in entities)
            {
                try
                {
                    //if (!entity.isActiveAndEnabled)
                    entity.Init();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        public void BeginNewGame()
        {
            ////GameStateController.Instance.NewGame();
            ////GameStateController.Instance.StartInit();
            ////MessageDispatcher2.Instance.DispatchMsg("SetGold", 0f, this.UniqueId, "UI Manager", GameDataController.Instance.Data.Inventory.Gold);
            ////var sceneConfig = GameDataController.Instance.NextSceneConfig;
            ////if (sceneConfig == null)
            ////    sceneConfig = GameDataController.Instance.CurrentSceneConfig;

            //MessageDispatcher2.Instance.DispatchMsg("SetHUDSkill", 0f, this.UniqueId, "UI Manager", null);
            // Populate starting items

            //Debug.Log("Populating " + sceneConfig.StartingItems.Length + " Starting Items");
            if (SceneConfig.StartingItems != null)
            {
                bool isFirst = true;
                //Debug.Log("Adding Starting Items to inventory.");
                foreach (var startingItem in SceneConfig.StartingItems)
                {
                    if (startingItem.ItemConfig == null)
                        continue;
                    //var addItemData = new ItemAndQuantityData()
                    //{
                    //    ItemConfig = startingItem.Item as IItemConfig,
                    //    Quantity = startingItem.Quantity
                    //};
                    MessageDispatcher.Instance.DispatchMsg("AddItem", 0f, this.GetInstanceID().ToString(), _player.GetId(),
                        startingItem);
                    //if (isFirst)
                    //{
                    //    GameDataController.Instance.Data.SelectedSkill = startingItem.Item.UniqueId;
                    //    MessageDispatcher2.Instance.DispatchMsg("SetHUDSkill", 0f, UniqueId, "UI Manager", startingItem.Item.UniqueId);
                    //    isFirst = false;
                    //}
                }

                var molds = SceneConfig.StartingItems.Where(i => i.ItemConfig.ItemType == ItemTypeEnum.Mold).ToList();
                var shards = SceneConfig.StartingItems.Where(i => i.ItemConfig.ItemType == ItemTypeEnum.Shard).ToList();

                if (shards.Count != 0)
                    GameStateController.Instance.CurrentShard = (ShardConfig)shards[0].ItemConfig;

                // TODO - This is broken as it assumes the shards apply to this Mold. FIX!
                //var moldData = new MoldData()
                //{
                //    MoldConfig = mold?.ItemConfig as MoldConfig,
                //    shardConfigs = shards?.ToList()
                //};

                MessageDispatcher.Instance.DispatchMsg("SetMoldData", 0f, string.Empty, "Hud Controller", molds);
                MessageDispatcher.Instance.DispatchMsg("SetShardData", 0f, string.Empty, "Hud Controller", shards);

                if (molds.Count > 0)
                {
                    Debug.Log("Setting Mold to " + molds[0].ItemConfig.name);
                    GameStateController.Instance.CurrentMold = molds[0].ItemConfig as MoldConfig;
                }
                //MessageDispatcher.Instance.DispatchMsg("SetMoldData", 0f, string.Empty, "Hud Controller", moldData);

                // Extract first starting blueprint and mold and set as active
                //MessageDispatcher.Instance.DispatchMsg("DisplayActiveMold", 0f, string.Empty, _player.GetId(), null);
                //for (int i = 0; i < SceneConfig.StartingItems.Length; i++)
                //{
                //    var startingItem = SceneConfig.StartingItems[i].ItemConfig;
                //    if (startingItem == null)
                //        continue;
                //    if (startingItem.ItemType == ItemTypeEnum.Mold)
                //        GameDataController.Instance.CurrentBlueprint = startingItem;
                //    if (startingItem.ItemClass == ItemClass.Mold)
                //    {
                //        Debug.Log("Setting Mold");
                //        MessageDispatcher2.Instance.DispatchMsg("SetMold", 0f, this.UniqueId, "UI Manager", startingItem);
                //    }
                //    //GameDataController.Instance.CurrentMold = startingItem;
                //}
            }
        }

        private void PlacePlayerAtSpawnPoint()
        {
            if (!String.IsNullOrEmpty(GameStateController.Instance.SpawnpointUniqueId))
            {
                _spawnPointComponents = GetSpawnPoints();
                //var sceneSetup = GameController.Instance.GetSceneSetup();

                //for (int i = 0; i < sceneSetup.SpawnPointComponents.Count; i++)
                //{

                //}
                //var spawnPoints = sceneSetup.SpawnPointComponents.Where(i =>
                //i.SpawnPointUniqueId == GameDataController.Instance.Data.SpawnpointUniqueId);

                PlaceEntityAtSpawnPoint(_spawnPointComponents);
                //PlaceEntityAtSpawnPoint(_spawnPointComponents, Enums.EntityType.Companion);
                GameStateController.Instance.SpawnpointUniqueId = null;
            }
        }

        private void PlaceEntityAtSpawnPoint(SpawnPointComponent[] spawnPoints)
        {
            SpawnPointComponent spawnPoint = null;

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i].SpawnPointUniqueId == GameStateController.Instance.SpawnpointUniqueId)
                {
                    spawnPoint = spawnPoints[i];
                    Debug.Log($"Located spawn point {spawnPoint.SpawnPointUniqueId}");
                    break;
                }
            }

            //var spawnPoint = spawnPoints.FirstOrDefault(i => i.EntityType == entityType);
            if (spawnPoint == null)
            {
                Debug.LogError($"(PlaceEntityAtSpawnPoint) Could not locate spawn point {GameStateController.Instance.SpawnpointUniqueId}.");
                return;
            }

            var pos = spawnPoint.transform.position;

            var player = EntityContainer.Instance.GetMainCharacter();
            if (player == null)
            {
                Debug.LogWarning($"(PlaceEntityAtSpawnPoint) Could not locate player.");
                return;
            }

            player.transform.position = pos;

            //string uniqueId = player;

            // Place the main character at the spawn point
            //var spawnPoint = GameData.Instance.CurrentScene.SpawnPoints[GameData.Instance.SpawnpointUniqueId];
            //MessageDispatcher.Instance.DispatchMsg(0f, this.UniqueId,
            //    uniqueId, Enums.Telegrams.SetPos,
            //    new Vector2D(pos.x, pos.y));
            //MessageDispatcher.Instance.DispatchMsg(0f, this.UniqueId, uniqueId,
            //    Enums.Telegrams.SetLevelHeight, spawnPoint.LevelLayer);


        }

        public SpawnPointComponent[] GetSpawnPoints()
        {
            if (SpawnPoints == null)
                return null;

            return SpawnPoints.GetComponentsInChildren<SpawnPointComponent>();

            //if (spawnPointComponents != null)
            //{
            //    SpawnPointComponents.AddRange(spawnPointComponents);
            //}
        }
    }
}

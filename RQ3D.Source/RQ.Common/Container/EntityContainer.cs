using System.Collections.Generic;
using UnityEngine;

namespace RQ.Common.Container
{
    public class EntityContainer : IEntityContainer
    {
        //[HideInInspector]
        //public Transform MainCharacter { get; set; }
        //public Transform CompanionCharacter { get; set; }
        private IEntity MainCharacter { get; set; }
        private IEntity CompanionCharacter { get; set; }
        private Dictionary<string, IEntity> Bosses { get; set; }
        //const string _idToCheck = "8da840b9-43f6-4bae-9e63-a3ceee1cb493";

        //[HideInInspector]
        /// <summary>
        /// Map instanceID to GameObjects.  It is a dictionary to make sure the same InstanceID does not get used twice (duplicate entity)
        /// </summary>
        //public Dictionary<string, Transform> EntityInstanceMap = new Dictionary<string, Transform>();
        public Dictionary<string, IEntity> EntityInstanceMap = new Dictionary<string, IEntity>();

        [HideInInspector]
        public static readonly EntityContainer Instance = new EntityContainer();

        private EntityContainer()
        {
            //Log.Info("EntityController instantiated");
            Bosses = new Dictionary<string, IEntity>();
        }

        public void AddEntity(IEntity entity)
        {
            //if (entity.UniqueId == _idToCheck)
            //{
            //    Debug.LogWarning("EntityContainer.AddEntity called on " + _idToCheck);
            //}
            if (EntityInstanceMap.ContainsKey(entity.GetId()))
                return;
            //    throw new Exception("EntityContainer - Unique Id " + entity.UniqueId + " already exists");

            // If the sprite already exists, exit
            //if (EntityInstanceMap.ContainsKey(entity.UniqueId))
            //    return;
            //if (SpriteInstanceMap.Contains(sprite))
            //    return;

            //string name;
            //name = entity.name;
            //if (entity.transform.parent != null)
            //    name = entity.transform.parent.name;
            //else
            //    name = entity.name;
            //Log.Info("Adding entity " + name);
            //Log.Info("Entity Count " + EntityInstanceMap.Count);
            switch (entity.tag)
            {
                case "Player":
                    SetMainCharacter(entity);
                    //MainCharacter = entity;   // This is not yet converted
                    break;
                case "Companion":
                    SetCompanionCharacter(entity);
                    //CompanionCharacter = entity;   // This is not yet converted
                    break;
                case "Boss":
                    SetBossCharacter(entity);
                    break;
                default:

                    break;
            }
            EntityInstanceMap.Add(entity.GetId(), entity);
            //SpriteInstanceMap.Add(sprite);

        }

        public void AddEntities(IList<IEntity> entities)
        {
            if (entities == null)
                return;
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                AddEntity(entity);
            }
            //foreach (var entity in entities)
            //{
                //if (entity.UniqueId == _idToCheck)
                //{
                //    Debug.LogWarning("EntityContainer.AddEntities called on " + _idToCheck);
                //}
                
            
        }

        public void SetMainCharacter(IEntity repo)
        {
            //Debug.LogError("Setting Main Character to " + repo.name + "(" + repo.UniqueId + ")");
            string name;
            if (repo == null)
                name = "(null)";
            else
                name = repo.name;
            //Debug.LogError("Setting Main Character to " + name);
            MainCharacter = repo;
        }

        public void SetCompanionCharacter(IEntity repo)
        {
            string name;
            if (repo == null)
                name = "(null)";
            else
                name = repo.name;
            //Log.Info("Setting Companion Character to " + name);
            CompanionCharacter = repo;
        }

        public void SetBossCharacter(IEntity repo)
        {
            string name;
            if (repo == null)
                name = "(null)";
            else
                name = repo.name;
            //Log.Info("Setting Boss Character to " + name);
            if (!Bosses.TryGetValue(repo.GetId(), out var boss))
                Bosses.Add(repo.GetId(), repo);
            else
                boss = repo;
            //CompanionCharacter = repo;
        }

        public IEntity GetEntity(string id)
        {
            EntityInstanceMap.TryGetValue(id, out var value);
            //if (!EntityInstanceMap.TryGetValue(id, out value))
            //    throw new Exception("EntityContainer - Cannot locate id " + id);
            return value;
        }

        //public IEnumerable<IEntity> GetEntitiesFromTag(string tag)
        //{
        //    List<IEntity> entities = new List<IEntity>();
        //    foreach (var entity in EntityInstanceMap)
        //    {
        //        if (entity.Value.GetTag() == tag)
        //            entities.Add(entity.Value);
        //    }
        //    //if (!EntityInstanceMap.TryGetValue(id, out value))
        //    //    throw new Exception("EntityContainer - Cannot locate id " + id);
        //    //return value;
        //    return entities;
        //}

        public IEntity GetEntityFromTag(string tag)
        {
            //List<IEntity> entities = new List<IEntity>();
            foreach (var entity in EntityInstanceMap)
            {
                if (entity.Value.tag == tag)
                    return entity.Value;
            }
            //if (!EntityInstanceMap.TryGetValue(id, out value))
            //    throw new Exception("EntityContainer - Cannot locate id " + id);
            //return value;
            return null;
        }

        //public IEnumerable<IEntity> GetEntitiesFromTags(params string[] tags)
        //{
        //    List<IEntity> entities = new List<IEntity>();
        //    foreach (var entity in EntityInstanceMap)
        //    {
        //        var entityTag = entity.Value.GetTag();
        //        for (int k = 0; k < tags.Length; k++)
        //        {
        //            if (tags[k] == entityTag)
        //                entities.Add(entity.Value);
        //        }
        //        //if (tags.Contains(entity.Value.GetTag()))
        //        //    entities.Add(entity.Value);
        //    }
        //    //if (!EntityInstanceMap.TryGetValue(id, out value))
        //    //    throw new Exception("EntityContainer - Cannot locate id " + id);
        //    //return value;
        //    return entities;
        //}

        public T GetEntity<T>(string id) where T : class
        {
            return GetEntity(id) as T;
            //return GetEntity(id).GetComponent<T>();
        }

        public Dictionary<string, IEntity> GetAllEntities()
        {
            return EntityInstanceMap;
        }

        public bool Contains(string id)
        {
            return EntityInstanceMap.ContainsKey(id);
        }

        //public IEnumerable<IEntity> GetEntitiesThatContainType<T>(string id)
        //{
        //    return EntityInstanceMap.Values.Where(i => i.GetComponent<T>() != null);
        //    //return (BaseObject)EntityInstanceMap[id];
        //}

        public void RemoveEntity(IEntity entity)
        {
            string name = entity.name;
            //if (entity.GetId() == _idToCheck)
            //{
            //    Debug.LogWarning("EntityContainer.RemoveEntity called on " + _idToCheck);
            //}
            //if (entity.transform.parent != null)
            //    name = entity.transform.parent.name;
            //else
            //    name = entity.name;
            //Log.Info("Removing entity " + name + "(" + entity.GetName() + ")");
            //Log.Info("Entity Count " + EntityInstanceMap.Count);
            switch (entity.tag)
            {
                case "Player":
                    //Debug.LogError("Setting Main Character to (null)");
                    MainCharacter = null;
                    break;
                case "Companion":
                    CompanionCharacter = null;
                    break;
                case "Boss":
                    if (Bosses.ContainsKey(entity.GetId()))
                        Bosses.Remove(entity.GetId());
                    break;
                default:

                    break;
            }
            if (EntityInstanceMap.ContainsKey(entity.GetId()))
                EntityInstanceMap.Remove(entity.GetId());
        }

        //public Transform GetMainCharacter()
        //{
        //    return MainCharacter;
        //}

        public IEntity GetMainCharacter()
        {
            return MainCharacter;
        }

        public IEntity GetCompanionCharacter()
        {
            return CompanionCharacter;
        }

        public Dictionary<string, IEntity> GetBosses()
        {
            return Bosses;
        }

        public void ResetEntityList()
        {
            //Log.Info("Resetting entity list");
            EntityInstanceMap.Clear();
            //SpriteInstanceMap = new Dictionary<int, SpriteInput>();
            SetMainCharacter(null);
            SetCompanionCharacter(null);
            Bosses.Clear();
            //MainCharacter = null;
            //CompanionCharacter = null;
            //Log.Info("Entity Count " + EntityInstanceMap.Count);
        }
    }
}

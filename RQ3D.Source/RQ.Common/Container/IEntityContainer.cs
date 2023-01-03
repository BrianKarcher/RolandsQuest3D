using System;
using System.Collections.Generic;
using System.Text;
using RQ.Common.Components;
using RQ.Messaging;

namespace RQ.Common.Container
{
    public interface IEntityContainer
    {
        void AddEntities(IList<IEntity> entities);
        void AddEntity(IEntity entity);
        bool Contains(string id);
        Dictionary<string, IEntity> GetAllEntities();
        Dictionary<string, IEntity> GetBosses();
        IEntity GetCompanionCharacter();
        //IEnumerable<IEntity> GetEntitiesFromTag(string tag);
        IEntity GetEntityFromTag(string tag);
        //IEnumerable<IEntity> GetEntitiesFromTags(params string[] tags);
        IEntity GetEntity(string id);
        T GetEntity<T>(string id) where T : class;
        IEntity GetMainCharacter();
        void RemoveEntity(IEntity entity);
        void ResetEntityList();
        void SetBossCharacter(IEntity repo);
        void SetCompanionCharacter(IEntity repo);
        void SetMainCharacter(IEntity repo);
    }
}

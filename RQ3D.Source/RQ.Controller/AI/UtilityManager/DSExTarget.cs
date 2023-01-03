﻿using System.Collections.Generic;
using System.Linq;
using RQ.Common;
using RQ.Common.Container;

namespace UtilityManager
{
    public class DSExTarget
    {
        private IDSE _dse;
        private IEntity _target;
        private int _targetUniqueId;
        private IDecisionContext _context;
        private IEntityContainer _entityContainer;

        public DSExTarget(IEntityContainer entityContainer)
        {
            _entityContainer = entityContainer;
        }

        public void CreateContext(IntelligenceDef intelligenceDef, IDSE dse, IEntity repo, string[] allyTags, string[] enemyTags, IEntity target)
        {
            _target = target;
            if (target != null)
                _targetUniqueId = target.GetInstanceID();
            _dse = dse;
            var dc = new DecisionContext();
            dc.Self = new AICharacter(repo, intelligenceDef);
            if (allyTags != null && allyTags.Length != 0)
            {
                var ally = _entityContainer.GetEntityFromTag(allyTags[0]);
                dc.AllyEntity = new AICharacter(ally, null);
            }
            if (enemyTags != null && enemyTags.Any())
            {
                if (dc.EnemyEntities == null)
                    dc.EnemyEntities = new List<IAICharacter>();
                dc.EnemyEntities.Clear();
                for (int i = 0; i < enemyTags.Length; i++)
                {
                    var enemyTag = enemyTags[i];
                    var enemy = _entityContainer.GetEntityFromTag(enemyTag);
                    var aiCharacter = new AICharacter(enemy, null);
                    dc.EnemyEntities.Add(aiCharacter);
                }
            }
            if (target != null)
            {
                dc.EnemyEntity = new AICharacter(target, null);                
            }
            _context = dc;
        }

        public IDecisionContext GetContext()
        {
            return _context;
        }

        public IDSE GetDSE()
        {
            return _dse;
        }

        public IEntity GetTarget()
        {
            return _target;
        }
    }
}

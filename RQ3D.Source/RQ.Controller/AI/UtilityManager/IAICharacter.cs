using RQ.Common.Container;
using UnityEngine;

namespace UtilityManager
{
    public interface IAICharacter
    {
        IntelligenceDef IntelligenceDef { get; }
        float GetHealthCurrent();
        float GetHealthMax();
        IEntity Repo { get; }
        Vector2 GetPos();
        bool IsAlive();
        //void SetRepo(IComponentRepository repo);
        //IComponentRepository GetRepo();
    }
}
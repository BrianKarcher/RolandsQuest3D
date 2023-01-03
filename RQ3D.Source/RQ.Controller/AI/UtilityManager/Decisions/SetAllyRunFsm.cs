namespace UtilityManager.Decisions
{
    public class SetAllyRunFsm : RunFsm
    {
        public override void Start(IDecisionContext context, IDSE dse)
        {
            var self = context.Self.Repo;
            self.Target = context.AllyEntity.Repo.gameObject;

            //var aiComponent = context.Self.Repo.Components.GetComponent<AIComponent>();
            //aiComponent.Target = context.AllyEntity.Repo.transform;

            base.Start(context, dse);
        }
    }
}

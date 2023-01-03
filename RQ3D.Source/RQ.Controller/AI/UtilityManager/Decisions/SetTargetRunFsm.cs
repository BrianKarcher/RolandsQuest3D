namespace UtilityManager.Decisions
{
    public class SetTargetRunFsm : RunFsm
    {
        public override void Start(IDecisionContext context, IDSE dse)
        {
            var self = context.Self.Repo;
            self.Target = context.EnemyEntity.Repo.gameObject;

            base.Start(context, dse);
        }
    }
}

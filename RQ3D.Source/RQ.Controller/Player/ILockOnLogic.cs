namespace RQ.Controller.Player
{
    public interface ILockOnLogic
    {
        bool IsStrafePressed();
        void ProcessButtonDown();
        void ProcessButtonUp();
        bool IsATargetLocked();
    }
}
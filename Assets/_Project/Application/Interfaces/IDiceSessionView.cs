namespace _Project.Application.Interfaces
{
    public interface IDiceSessionView
    {
        void SetEnemyInfo(string enemyNameText);
        void SetEnemyHealth(string enemyHealthText);
        void SetCycleInfo(string cycleText);
        void SetTurnInfo(string turnText);
        void SetResultInfo(string resultText);
        void SetDealButtonInteractable(bool isInteractable);
        void SetDicePanelVisible(bool isVisible);
    }
}

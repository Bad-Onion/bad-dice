using System.Linq;
using _Project.Domain.Entities;
using Zenject;

namespace _Project.Infrastructure.Services
{
    public class GameInitializationService : IInitializable
    {
        private readonly DiceSession _diceSession;
        private readonly PlayerRunState _playerRunState;

        public GameInitializationService(DiceSession diceSession, PlayerRunState playerRunState)
        {
            _diceSession = diceSession;
            _playerRunState = playerRunState;
        }

        public void Initialize()
        {
            _diceSession.ActiveDice.Clear();

            var equippedDice = _playerRunState.Inventory.Where(d => d.IsEquipped).ToList();

            foreach (var ownedDice in equippedDice)
            {
                _diceSession.ActiveDice.Add(new DiceState
                {
                    Id = ownedDice.Id,
                    Definition = ownedDice.Definition,
                    Level = ownedDice.Level,
                    CurrentFaceIndex = -1,
                    IsSelectedForReroll = false
                });
            }
        }
    }
}
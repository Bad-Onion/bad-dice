using _Project.Domain.Features.Dice.Entities;

namespace _Project.Application.Interfaces
{
    public interface IDiceDamageService
    {
        int CalculateDamage(DiceState dice);
    }
}
using _Project.Domain.Entities;

namespace _Project.Application.Interfaces
{
    public interface IDiceDamageService
    {
        int CalculateDamage(DiceState dice);
    }
}
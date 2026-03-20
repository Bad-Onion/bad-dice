using _Project.Domain.Entities;
using _Project.Domain.Entities.DiceData;

namespace _Project.Application.Interfaces
{
    public interface IDiceDamageService
    {
        int CalculateDamage(DiceState dice);
    }
}
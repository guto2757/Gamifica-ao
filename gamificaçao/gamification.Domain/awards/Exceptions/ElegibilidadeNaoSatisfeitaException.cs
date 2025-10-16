using System;

namespace Gamification.Domain.Awards.Exceptions;

public class ElegibilidadeNaoSatisfeitaException : Exception
{
    public ElegibilidadeNaoSatisfeitaException(string message) : base(message) { }
}

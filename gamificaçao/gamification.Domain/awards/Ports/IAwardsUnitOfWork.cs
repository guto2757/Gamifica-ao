using System;

namespace Gamification.Domain.Awards.Ports;

public interface IAwardsUnitOfWork : IDisposable
{
    void Commit();
    void Rollback();
}
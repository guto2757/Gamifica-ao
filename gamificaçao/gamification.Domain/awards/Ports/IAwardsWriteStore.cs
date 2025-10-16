using System;
using Gamification.Domain.Model;

namespace Gamification.Domain.Awards.Ports;

public interface IAwardsWriteStore
{
    /// <summary>
    /// Persiste a concessão de badge, XP (opcional) e log de auditoria de forma atômica.
    /// </summary>
    void SalvarConcessaoAtomica(Badge badge, XpAmount? xp, RewardLog log);
}

using System;

namespace Gamification.Domain.Awards.Ports;

public interface IAwardsReadStore
{
    /// <summary>
    /// Indica se a missão foi concluída pelo estudante.
    /// </summary>
    bool MissaoConcluida(Guid studentId, Guid missionId);

    /// <summary>
    /// Indica se a badge já foi concedida (idempotência / unicidade).
    /// </summary>
    bool JaConcedeuBadge(Guid studentId, Guid missionId, string badgeSlug);

    /// <summary>
    /// Retorna as janelas de bônus para a missão: (start, fullEnd, finalEnd).
    /// </summary>
    (DateTimeOffset bonusStartDate, DateTimeOffset bonusFullWeightEndDate, DateTimeOffset bonusFinalDate) ObterJanelasDeBonus(Guid missionId);
}

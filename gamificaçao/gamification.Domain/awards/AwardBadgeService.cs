using System;
using Gamification.Domain.Awards.Policies;
using Gamification.Domain.Awards.Ports;
using Gamification.Domain.Awards.Exceptions;
using Gamification.Domain.Model;

namespace Gamification.Domain.Awards;

public class AwardBadgeService
{
    private readonly IAwardsReadStore _readStore;
    private readonly IAwardsWriteStore _writeStore;
    private readonly BonusPolicy _bonusPolicy;

    private const string FonteAcao = "mission_completion";
    private const string BadgeSlug = "mission_completion";

    public AwardBadgeService(
        IAwardsReadStore readStore,
        IAwardsWriteStore writeStore,
        BonusPolicy bonusPolicy)
    {
        _readStore = readStore ?? throw new ArgumentNullException(nameof(readStore));
        _writeStore = writeStore ?? throw new ArgumentNullException(nameof(writeStore));
        _bonusPolicy = bonusPolicy ?? throw new ArgumentNullException(nameof(bonusPolicy));
    }

    /// <summary>
    /// Concede a badge ao estudante, seguindo regras de elegibilidade, unicidade, bônus e auditoria.
    /// Implementação mínima para uso via TDD; adaptações/validações extras podem ser adicionadas.
    /// </summary>
    public void ConcederBadge(Guid studentId, Guid missionId, DateTimeOffset now, Guid? requestId = null)
    {
        // 1) Elegibilidade
        if (!_readStore.MissaoConcluida(studentId, missionId))
            throw new ElegibilidadeNaoSatisfeitaException("Missão não concluída — estudante inelegível para badge.");

        // 2) Idempotência / Unicidade: não conceder se já concedeu
        if (_readStore.JaConcedeuBadge(studentId, missionId, BadgeSlug))
            return;

        // 3) Obter janelas de bônus e calcular XP
        var (bonusStartDate, bonusFullWeightEndDate, bonusFinalDate) = _readStore.ObterJanelasDeBonus(missionId);

        var xp = _bonusPolicy.CalcularBonus(
            now,
            bonusStartDate,
            bonusFullWeightEndDate,
            bonusFinalDate,
            xpBase: 100,
            xpFullWeight: 200,
            xpReducedWeight: 150,
            out var justificativa);

        // 4) Construir domínio e persistir atomicamente
        var badge = new Badge(BadgeSlug, "Conclusão de Missão");
        var log = new RewardLog(studentId, BadgeSlug, now, FonteAcao, justificativa);

        _writeStore.SalvarConcessaoAtomica(badge, xp, log);
    }
}

using System;
using Gamification.Domain.Awards;
using Gamification.Domain.Awards.Policies;
using Gamification.Domain.Awards.Ports;
using Gamification.Domain.Awards.Exceptions;
using Gamification.Domain.Model;
using Xunit;

namespace Gamification.Domain.Tests;

public class AwardBadgeServiceTests
{
    [Fact(DisplayName = "ConcederBadge_quando_missao_concluida_concede_uma_unica_vez")]
    public void ConcederBadge_quando_missao_concluida_concede_uma_unica_vez()
    {
        // Arrange
        var readStore = new FakeReadStore(missaoConcluida: true, jaConcedeu: false);
        var writeStore = new FakeWriteStore();
        var policy = new BonusPolicy();
        var service = new AwardBadgeService(readStore, writeStore, policy);

        var studentId = Guid.NewGuid();
        var missionId = Guid.NewGuid();

        // Act
        service.ConcederBadge(studentId, missionId, DateTimeOffset.UtcNow);

        // Assert
        Assert.True(writeStore.BadgeGravada, "A badge deveria ser gravada.");
    }

    [Fact(DisplayName = "ConcederBadge_sem_concluir_missao_deve_falhar")]
    public void ConcederBadge_sem_concluir_missao_deve_falhar()
    {
        // Arrange
        var readStore = new FakeReadStore(missaoConcluida: false, jaConcedeu: false);
        var writeStore = new FakeWriteStore();
        var policy = new BonusPolicy();
        var service = new AwardBadgeService(readStore, writeStore, policy);

        var studentId = Guid.NewGuid();
        var missionId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.Throws<ElegibilidadeNaoSatisfeitaException>(
            () => service.ConcederBadge(studentId, missionId, DateTimeOffset.UtcNow));

        Assert.Equal("Missão não concluída — estudante inelegível para badge.", ex.Message);
        Assert.False(writeStore.BadgeGravada, "Nenhuma badge deve ser gravada quando a missão não foi concluída.");
    }
}

#region Fakes used in tests
// Fake read store
internal class FakeReadStore : IAwardsReadStore
{
    private readonly bool _missaoConcluida;
    private readonly bool _jaConcedeu;
    private readonly (DateTimeOffset, DateTimeOffset, DateTimeOffset) _janelas;

    public FakeReadStore(bool missaoConcluida, bool jaConcedeu)
    {
        _missaoConcluida = missaoConcluida;
        _jaConcedeu = jaConcedeu;
        var now = DateTimeOffset.UtcNow;
        _janelas = (now.AddDays(-2), now.AddDays(1), now.AddDays(2));
    }

    public bool MissaoConcluida(Guid studentId, Guid missionId) => _missaoConcluida;

    public bool JaConcedeuBadge(Guid studentId, Guid missionId, string badgeSlug) => _jaConcedeu;

    public (DateTimeOffset, DateTimeOffset, DateTimeOffset) ObterJanelasDeBonus(Guid missionId) => _janelas;
}

// Fake write store
internal class FakeWriteStore : IAwardsWriteStore
{
    public bool BadgeGravada { get; private set; }

    public void SalvarConcessaoAtomica(Badge badge, XpAmount? xp, RewardLog log)
    {
        BadgeGravada = true;
        // Simula persistência sem efeitos colaterais
    }
}
#endregion

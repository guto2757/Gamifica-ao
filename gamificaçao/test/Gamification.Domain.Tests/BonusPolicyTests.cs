using System;
using Gamification.Domain.Awards.Policies;
using Xunit;

namespace Gamification.Domain.Tests;

public class BonusPolicyTests
{
    [Fact(DisplayName = "ConcederBadge_ate_bonusFullWeightEndDate_concede_bonus_integral")]
    public void ConcederBadge_ate_bonusFullWeightEndDate_concede_bonus_integral()
    {
        var policy = new BonusPolicy();
        var now = DateTimeOffset.UtcNow;
        var start = now.AddDays(-2);
        var full = now.AddDays(1);
        var final = now.AddDays(2);

        var result = policy.CalcularBonus(now, start, full, final, 100, 200, 150, out var reason);

        Assert.Equal(200, result.Value);
        Assert.Equal("janela integral", reason);
    }
}
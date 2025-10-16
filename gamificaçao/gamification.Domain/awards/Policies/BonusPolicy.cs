using System;
using Gamification.Domain.Model;

namespace Gamification.Domain.Awards.Policies;

public class BonusPolicy
{
    /// <summary>
    /// Calcula o XP de bônus com base nas janelas de tempo.
    /// Retorna um XpAmount e preenche justificativa com o motivo.
    /// </summary>
    public XpAmount CalcularBonus(
        DateTimeOffset now,
        DateTimeOffset bonusStartDate,
        DateTimeOffset bonusFullWeightEndDate,
        DateTimeOffset bonusFinalDate,
        int xpBase,
        int xpFullWeight,
        int xpReducedWeight,
        out string justificativa)
    {
        // Validação mínima de configuração:
        if (bonusStartDate > bonusFullWeightEndDate || bonusFullWeightEndDate > bonusFinalDate)
            throw new ArgumentException("Configuração de janelas de bônus inválida.");

        if (now <= bonusFullWeightEndDate)
        {
            justificativa = "janela integral";
            return new XpAmount(xpFullWeight);
        }

        if (now <= bonusFinalDate)
        {
            justificativa = "janela reduzida";
            return new XpAmount(xpReducedWeight);
        }

        justificativa = "fora da janela de bônus";
        return XpAmount.Zero;
    }
}

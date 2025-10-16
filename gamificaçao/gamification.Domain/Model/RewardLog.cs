using System;

namespace Gamification.Domain.Model;

public record RewardLog(Guid StudentId, string BadgeSlug, DateTimeOffset AwardedAt, string Source, string Reason);
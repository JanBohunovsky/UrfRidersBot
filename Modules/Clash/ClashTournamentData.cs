using LiteDB;
using MingweiSamuel.Camille.ClashV1;
using System;
using System.Globalization;
using System.Linq;

namespace UrfRiders.Modules.Clash
{
    public class ClashTournamentData
    {
        [BsonId]
        public int TournamentId { get; set; }
        public int ScheduleId { get; set; }

        public string Name { get; set; }
        public string SecondaryName { get; set; }
        public DateTimeOffset RegistrationTime { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public bool Cancelled { get; set; }

        // ◇ ● ◈ ◆ ▪
        public string FormattedName => $"{Name} Cup ◇ {SecondaryName}";
        public string FormattedTime => $"{StartTime:d MMMM} ◇ {RegistrationTime:HH:mm} - {StartTime:HH:mm}";

        public static ClashTournamentData Parse(Tournament tournament)
        {
            var schedule = tournament.Schedule.FirstOrDefault();
            if (schedule == null)
                throw new Exception("Tournament does not have schedule.");

            return new ClashTournamentData
            {
                TournamentId = tournament.Id,
                ScheduleId = schedule.Id,
                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tournament.NameKey.Replace("_", " ")),
                SecondaryName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tournament.NameKeySecondary.Replace("_", " ")),
                RegistrationTime = DateTimeOffset.FromUnixTimeMilliseconds(schedule.RegistrationTime).ToLocalTime(),
                StartTime = DateTimeOffset.FromUnixTimeMilliseconds(schedule.StartTime).ToLocalTime(),
                Cancelled = schedule.Cancelled
            };
        }

    }
}
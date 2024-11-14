using System;

namespace BackdoorGuard;

public record OutingSchedule(
    long ScheduleId, 
    string Title,
    DateTime StartTime, 
    DateTime EndTime
);
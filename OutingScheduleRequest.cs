using System;

namespace BackdoorGuard;

public record OutingScheduleRequest (
    string title,
    string outingStartTime, 
    string outingEndTime
);
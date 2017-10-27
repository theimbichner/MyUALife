using System;
namespace MyUALife
{
    public class Model
    {
        public static Calendar Calendar
        {
            get;
        }

        static Model()
        {
            Calendar = new Calendar();

            DateTime time = DateTime.Now;
            DateTime midnightMorning = DateTime.Today;
            DateTime midnightNight = DateTime.Today.AddDays(1);

            String classDesc       = "Software Engineering";
            String appointmentDesc = "This is an appointment with a professor.";
            String hwDesc          = "This is homework for CS436";
            String studyDesc       = "This is study time.";
            String recDesc         = "This is a recreational event.";
            String freeDesc        = "This is free time. This category hould not appear in user added events.";
            String lastNightDesc   = "This was midnight last night. This should appear as part of yesterday.";
            String tonightDesc     = "This is midnight tonight. This should appear as part of today.";

            Calendar.AddEvent(new Event("CS436 Class",       classDesc,       Category.classTime,   time, time));
            Calendar.AddEvent(new Event("Office Hours",      appointmentDesc, Category.appointment, time, time));
            Calendar.AddEvent(new Event("Build Android App", hwDesc,          Category.homework,    time, time));
            Calendar.AddEvent(new Event("Study",             studyDesc,       Category.studyTime,   time, time));
            Calendar.AddEvent(new Event("Play Videogames",   recDesc,         Category.recreation,  time, time));
            Calendar.AddEvent(new Event("???",               freeDesc,        Category.freeTime,    time, time));
            Calendar.AddEvent(new Event("Last Midnight",     lastNightDesc,   Category.freeTime,    midnightMorning, midnightMorning));
            Calendar.AddEvent(new Event("Midnight Tonight",  tonightDesc,     Category.freeTime,    midnightNight, midnightNight));

            Calendar.AddDeadline(new Deadline("Deadline", "It's a deadline!!!!!!!!!!", time));
        }
    }
}
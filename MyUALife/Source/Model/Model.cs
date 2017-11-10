using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace MyUALife
{
    public class Model
    {
        private const String fileName = "calendar_save_state.bin";

        public static Calendar Calendar
        {
            get;
        }

        static Model()
        {
            if (File.Exists(fileName))
            {
                Stream fileStream = File.OpenRead(fileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                Calendar = (Calendar)deserializer.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                Calendar = new Calendar();
            }

            DateTime time = DateTime.Now;
            DateTime midnightMorning = DateTime.Today;
            DateTime midnightNight = DateTime.Today.AddDays(1);

            String classDesc       = "Software Engineering";
            String appointmentDesc = "This is an appointment with a professor.";
            String hwDesc          = "This is homework for CS436";
            String studyDesc       = "This is study time.";
            String recDesc         = "This is a recreational event.";
            String freeDesc        = "This is free time. This category should not appear in user added events.";
            String lastNightDesc   = "This was midnight last night. This should appear as part of yesterday.";
            String tonightDesc     = "This is midnight tonight. This should appear as part of today.";

            Calendar.AddEvent(new Event("CS436 Class",       classDesc,       Category.classTime,   Time(12, 30), Time(13, 45) ));
            Calendar.AddEvent(new Event("Office Hours",      appointmentDesc, Category.appointment, Time(14, 30), Time(15,  0) ));
            Calendar.AddEvent(new Event("Build Android App", hwDesc,          Category.homework,    Time(15, 15), Time(17,  0) ));
            Calendar.AddEvent(new Event("Study",             studyDesc,       Category.studyTime,   Time(10,  0), Time(12, 15) ));
            Calendar.AddEvent(new Event("Play Videogames",   recDesc,         Category.recreation,  Time(18, 25), Time(23, 59) ));
            // Calendar.AddEvent(new Event("???",               freeDesc,        Category.freeTime,    time, time));
            // Calendar.AddEvent(new Event("Last Midnight",     lastNightDesc,   Category.freeTime,    midnightMorning, midnightMorning));
            // Calendar.AddEvent(new Event("Midnight Tonight",  tonightDesc,     Category.freeTime,    midnightNight, midnightNight));

            Calendar.AddDeadline(new Deadline("Deadline", "It's a deadline!!!!!!!!!!", midnightNight));
        }

        static DateTime Time(int hours, int mins)
        {
            return DateTime.Today.AddHours(hours).AddMinutes(mins);
        }
    }
}
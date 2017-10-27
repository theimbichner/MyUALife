using System;

public class Deadline {

	public DateTime Time { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }

	public EventType associatedEventType = Category.other;

	public Deadline(string name, string desc, DateTime time) {
		Name = name;
		Description = desc;
		Time = time;
	}

	public Deadline(string name, string desc, DateTime time, EventType eventType) {
        Name = name;
        Description = desc;
        Time = time;
        associatedEventType = eventType;
	}

    public override string ToString()
    {
        string format = "Name: {0}\nDescription: {1}\nTime: {2}";
        object[] args = { Name, Description, Time };
        return string.Format(format, args);
    }

}
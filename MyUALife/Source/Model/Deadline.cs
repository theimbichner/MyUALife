using System;

public class Deadline {

	public DateTime Time { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }

	EventType associatedEventType = Category.other;

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

}
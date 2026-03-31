// Models/Event.cs

namespace EventEase.Models
{
    // This class represents one event in our app.
    // It holds all the data fields we want to display.
    public class Event
    {
        // Each event has a unique ID so we can find it later (e.g., in URLs)
        public int Id { get; set; }

        // The name of the event, e.g., "Tech Summit 2025"
        public string Name { get; set; } = string.Empty;

        // When the event takes place
        public DateTime Date { get; set; }

        // Where the event is held
        public string Location { get; set; } = string.Empty;

        // A short description shown on the details page
        public string Description { get; set; } = string.Empty;

        // Maximum number of people who can attend
        public int Capacity { get; set; }

        // How many people have already registered
        public int RegisteredCount { get; set; }

        // A computed property: is there still space?
        // Properties with only a getter are called "read-only computed properties"
        public bool HasAvailability => RegisteredCount < Capacity;

    }
}
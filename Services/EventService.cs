// Services/EventService.cs

using EventEase.Models;

namespace EventEase.Services
{
    // This service provides event data to any component that needs it.
    // In a real app, this would talk to a database or API.
    public class EventService
    {
        // A private list holds our mock events.
        // "private" means only this class can modify the list directly.
        private readonly List<Event> _events = new()
        {
            new Event
            {
                Id = 1,
                Name = "Tech Summit 2025",
                Date = new DateTime(2025, 6, 15),
                Location = "Berlin, Germany",
                Description = "Annual technology conference featuring the latest in AI, cloud, and software development.",
                Capacity = 500,
                RegisteredCount = 342
            },
            new Event
            {
                Id = 2,
                Name = "Design & UX Expo",
                Date = new DateTime(2025, 7, 20),
                Location = "Munich, Germany",
                Description = "A deep-dive into modern UX design patterns, accessibility, and user research methods.",
                Capacity = 200,
                RegisteredCount = 198
            },
            new Event
            {
                Id = 3,
                Name = "Startup Weekend",
                Date = new DateTime(2025, 8, 5),
                Location = "Hamburg, Germany",
                Description = "54-hour event where developers, designers, and business people build startups together.",
                Capacity = 150,
                RegisteredCount = 60
            },
            new Event
            {
                Id = 4,
                Name = "Cloud Architecture Workshop",
                Date = new DateTime(2025, 9, 12),
                Location = "Frankfurt, Germany",
                Description = "Hands-on workshop covering AWS, Azure, and GCP architecture best practices.",
                Capacity = 80,
                RegisteredCount = 80  // This one is FULL — good for testing HasAvailability
            }
        };

        // Returns ALL events — used by the EventList page
        public List<Event> GetAllEvents() => _events;

        // Returns ONE event by its ID — used by the EventDetails page
        // The "?" after Event means the return value can be null (if not found)
        public Event? GetEventById(int id) => _events.FirstOrDefault(e => e.Id == id);

        // Registers one more person for an event
        // Returns false if the event is full or doesn't exist
        public bool RegisterForEvent(int eventId)
        {
            var ev = GetEventById(eventId);
            if (ev == null || !ev.HasAvailability) return false;

            ev.RegisteredCount++;
            return true;
        }
    }
}
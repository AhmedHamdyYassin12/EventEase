// Services/SessionService.cs

using EventEase.Models;

namespace EventEase.Services
{
    // This service stores the current user's session data.
    // Because it's Scoped (registered in Program.cs), one instance exists
    // per user connection — it persists across page navigations.
    public class SessionService
    {
        // The currently logged-in / registered user's details
        // Null means nobody has registered yet in this session
        public Registration? CurrentUser { get; private set; }

        // List of event IDs the user has registered for in this session
        public List<int> RegisteredEventIds { get; private set; } = new();

        // Called after successful registration
        public void SetUser(Registration registration)
        {
            CurrentUser = registration;
            if (!RegisteredEventIds.Contains(registration.EventId))
            {
                RegisteredEventIds.Add(registration.EventId);
            }
        }

        // Check if the user has already registered for a specific event
        public bool IsRegisteredFor(int eventId) => RegisteredEventIds.Contains(eventId);

        // Clear the session (logout / reset)
        public void ClearSession()
        {
            CurrentUser = null;
            RegisteredEventIds.Clear();
        }
    }
}
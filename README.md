# EventEase – Complete Blazor Learning Project
### All 3 Phases with Full Code & Explanations

---

## 📁 Project Structure Overview

```
EventEase/
├── Models/
│   ├── Event.cs                  ← Data model for an event
│   └── Registration.cs           ← Data model for user registration
├── Services/
│   ├── EventService.cs           ← Mock data & event logic
│   └── SessionService.cs         ← User session state (Activity 3)
├── Pages/
│   ├── EventList.razor           ← Homepage listing all events
│   ├── EventDetails.razor        ← Details page for a single event
│   ├── Register.razor            ← Registration form page
│   └── NotFound.razor            ← 404 fallback page
├── Components/
│   └── EventCard.razor           ← Reusable event card component
├── Shared/
│   ├── NavMenu.razor             ← Navigation bar
│   └── MainLayout.razor          ← App shell layout
├── App.razor                     ← Root component + routing setup
└── Program.cs                    ← App entry point
```

---

## ⚙️ SETUP: Create the Project

In Visual Studio:
1. **File → New → Project**
2. Choose **"Blazor Server App"**
3. Name it **EventEase**
4. Target **.NET 10**
5. Click **Create**

---

---

# 🔵 ACTIVITY 1 — Foundations: Components, Data Binding & Routing

---

## 1. Models/Event.cs

> **What is a Model?**
> A model is a plain C# class that defines the *shape* of your data.
> Think of it as a blueprint — it says "an Event has a Name, a Date, and a Location."

```csharp
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
```

---

## 2. Services/EventService.cs

> **What is a Service?**
> A service is a class that holds *business logic* and *data operations*.
> Instead of putting data directly in components, we centralize it here.
> This makes it easy to swap mock data for a real database later.
>
> **Dependency Injection (DI):** Blazor can automatically give components
> access to services — this is called "injecting" the service.

```csharp
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
```

---

## 3. Components/EventCard.razor

> **What is a Component?**
> A Blazor component is a reusable piece of UI defined in a `.razor` file.
> It has three sections:
> - **HTML markup** — what it looks like
> - **@code block** — its C# logic
> - **Parameters** — data passed in from a parent component
>
> Think of a component like a custom HTML tag you define yourself.

```razor
@* Components/EventCard.razor *@

@* We import the Models namespace so we can use the Event type *@
@using EventEase.Models

@* ───────────────────────────────────────────
   HTML SECTION — Defines the visual structure
   ─────────────────────────────────────────── *@

<div class="event-card @(CurrentEvent.HasAvailability ? "" : "event-card--full")">

    @* Event name as a heading *@
    <h3 class="event-card__title">@CurrentEvent.Name</h3>

    @* 📅 Date — formatted nicely with C# string formatting *@
    <p class="event-card__date">
        📅 @CurrentEvent.Date.ToString("MMMM dd, yyyy")
    </p>

    @* 📍 Location *@
    <p class="event-card__location">
        📍 @CurrentEvent.Location
    </p>

    @* Availability badge — conditional rendering with @if *@
    @if (CurrentEvent.HasAvailability)
    {
        <span class="badge badge--available">
            @CurrentEvent.RegisteredCount / @CurrentEvent.Capacity spots filled
        </span>
    }
    else
    {
        <span class="badge badge--full">FULLY BOOKED</span>
    }

    @* NavigationLink — routes to the details page for this specific event *@
    @* The href uses the event's Id to build the URL, e.g., /event/1 *@
    <a href="/event/@CurrentEvent.Id" class="btn btn--primary">View Details</a>
</div>

@* ───────────────────────────────────────────
   CODE SECTION — C# logic for this component
   ─────────────────────────────────────────── *@
@code {
    // [Parameter] means this value is PASSED IN from a parent component.
    // The parent writes: <EventCard CurrentEvent="someEvent" />
    // Without [Parameter], the parent can't set this property.
    [Parameter]
    public Event CurrentEvent { get; set; } = new Event();
}
```

---

## 4. Pages/EventList.razor

> **What is a Page?**
> A page is a component with a `@page` directive — this gives it a URL route.
> When users navigate to that URL, Blazor renders this component.
>
> **@inject** pulls a service into the component via Dependency Injection.
> **@foreach** loops through a list to render repeated UI elements.

```razor
@* Pages/EventList.razor *@
@page "/"   @* This page is the homepage, accessible at "/" *@

@using EventEase.Models
@using EventEase.Components

@* Inject our EventService — Blazor will provide the instance automatically *@
@inject EventEase.Services.EventService EventService

<h1>Upcoming Events</h1>
<p>Browse our upcoming events and register for the ones you're interested in.</p>

@* ── LOADING STATE ──
   @if checks a boolean. While events are loading (in a real app), show a spinner. *@
@if (_events == null)
{
    <p>Loading events...</p>
}
else if (!_events.Any())
{
    <p>No events found.</p>
}
else
{
    @* Loop through each event and render an EventCard component *@
    <div class="event-grid">
        @foreach (var ev in _events)
        {
            @* Pass the event object into the EventCard via the [Parameter] we defined *@
            <EventCard CurrentEvent="ev" />
        }
    </div>
}

@code {
    // Private field to hold the list of events
    // The "?" means it can be null initially (before data is loaded)
    private List<Event>? _events;

    // OnInitializedAsync runs when the component first loads
    // This is a Blazor lifecycle method — think of it as "on page load"
    protected override async Task OnInitializedAsync()
    {
        // In a real app, you'd await an HTTP call here.
        // We use Task.FromResult to simulate async behavior with mock data.
        _events = await Task.FromResult(EventService.GetAllEvents());
    }
}
```

---

## 5. Pages/EventDetails.razor

> **Route Parameters:**
> `@page "/event/{Id:int}"` captures the number in the URL.
> If the URL is `/event/3`, then `Id` will be `3`.
> The `:int` part is a *route constraint* — it only matches whole numbers.

```razor
@* Pages/EventDetails.razor *@
@page "/event/{Id:int}"   @* Matches URLs like /event/1, /event/2, etc. *@

@using EventEase.Models
@inject EventEase.Services.EventService EventService
@inject NavigationManager NavManager  @* For navigating programmatically *@

@if (_event == null)
{
    @* Show this if the event ID doesn't exist *@
    <div class="not-found">
        <h2>Event not found</h2>
        <p>The event you're looking for doesn't exist.</p>
        <a href="/" class="btn btn--secondary">← Back to Events</a>
    </div>
}
else
{
    <div class="event-details">
        <a href="/" class="back-link">← All Events</a>

        <h1>@_event.Name</h1>

        <div class="event-details__meta">
            <span>📅 @_event.Date.ToString("dddd, MMMM dd, yyyy")</span>
            <span>📍 @_event.Location</span>
        </div>

        <p class="event-details__description">@_event.Description</p>

        <div class="event-details__capacity">
            <strong>Capacity:</strong> @_event.RegisteredCount / @_event.Capacity registered
        </div>

        @* Only show the Register button if there's space *@
        @if (_event.HasAvailability)
        {
            <a href="/register/@_event.Id" class="btn btn--primary">Register Now</a>
        }
        else
        {
            <button class="btn btn--disabled" disabled>Fully Booked</button>
        }
    </div>
}

@code {
    // [Parameter] here comes from the URL route, not a parent component
    // Blazor automatically fills this from the URL segment {Id:int}
    [Parameter]
    public int Id { get; set; }

    private Event? _event;

    protected override async Task OnInitializedAsync()
    {
        // Look up the event by the ID from the URL
        _event = await Task.FromResult(EventService.GetEventById(Id));
    }
}
```

---

## 6. Pages/NotFound.razor

> **Fallback routing:** When no route matches, Blazor falls back to this component.
> It's configured in `App.razor` (shown below).

```razor
@* Pages/NotFound.razor — shown when no route matches *@

<div class="not-found-page">
    <h1>404 — Page Not Found</h1>
    <p>Oops! The page you're looking for doesn't exist.</p>
    <a href="/" class="btn btn--primary">Go Home</a>
</div>
```

---

## 7. App.razor

> **App.razor is the root of your app.**
> It sets up the `<Router>` — the system that reads the URL and decides
> which page component to render.
> `<Found>` renders the matched page; `<NotFound>` renders the 404 page.

```razor
@* App.razor — Root component, handles routing *@

<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        @* RouteView renders the component that matches the current URL *@
        @* DefaultLayout wraps every page in MainLayout *@
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />

        @* FocusOnNavigate improves accessibility by moving focus on page change *@
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>

    <NotFound>
        @* When no route matches, show our custom 404 page *@
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <NotFound />
        </LayoutView>
    </NotFound>
</Router>
```

---

## 8. Shared/NavMenu.razor

```razor
@* Shared/NavMenu.razor — Navigation bar shown on every page *@

<nav class="navbar">
    <a href="/" class="navbar__brand">🎪 EventEase</a>

    <div class="navbar__links">
        @* NavLink is like <a> but it adds an "active" CSS class automatically
           when the current URL matches the href *@
        <NavLink href="/" Match="NavLinkMatch.All">Events</NavLink>
    </div>
</nav>
```

---

## 9. Shared/MainLayout.razor

> **Layout components** wrap every page in shared structure (nav, footer, etc.).
> `@Body` is where the current page's content is inserted.

```razor
@* Shared/MainLayout.razor — Wraps every page with nav + footer *@
@inherits LayoutComponentBase   @* Must inherit this for layouts *@

<NavMenu />   @* Render the nav bar at the top *@

<main class="main-content">
    @Body   @* The current page's content goes here *@
</main>

<footer class="footer">
    <p>© 2025 EventEase. All rights reserved.</p>
</footer>
```

---

## 10. Program.cs

> **Program.cs** bootstraps the application.
> We register our services here so Blazor's DI system can inject them.

```csharp
// Program.cs — App startup and service registration

using EventEase.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor Server services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// ─── Register our custom services ───
// AddScoped means: create one instance per user connection (per browser tab)
// Other options: AddSingleton (one for all users), AddTransient (new each time)
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<SessionService>();  // Added in Activity 3

var app = builder.Build();

// Standard middleware pipeline
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

---

---

# 🟡 ACTIVITY 2 — Debugging, Validation & Performance

---

## Changes in Activity 2

Activity 2 adds:
1. **Input validation** — prevent bad data from breaking the app
2. **Error boundaries** — gracefully catch rendering errors
3. **Virtualization** — efficiently render large lists (performance)
4. **404 handling** — already done above with the NotFound page

---

## Models/Registration.cs  (new in Activity 2)

> **Data Annotations** are attributes (like `[Required]`) that describe rules about data.
> Blazor's `<EditForm>` reads these to automatically validate form inputs.

```csharp
// Models/Registration.cs

using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Registration
    {
        // [Required] = this field must not be empty
        [Required(ErrorMessage = "Full name is required.")]
        // [StringLength] = limits how long the string can be
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        // [EmailAddress] = must look like a valid email
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        // [Phone] = must look like a phone number
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; } = string.Empty;

        // Which event this registration is for
        public int EventId { get; set; }

        // Computed from EventId — set when we process the registration
        public string EventName { get; set; } = string.Empty;
    }
}
```

---

## Updated Pages/EventList.razor — Virtualization

> **Virtualization** is a performance technique: instead of rendering ALL items in the DOM
> (which is slow for 1000+ items), Blazor only renders what's VISIBLE on screen.
> As you scroll, it swaps items in and out. This makes large lists feel instant.

```razor
@* Pages/EventList.razor — Updated with search filter and virtualization *@
@page "/"
@using EventEase.Models
@using EventEase.Components
@using Microsoft.AspNetCore.Components.Web.Virtualization  @* Import virtualization *@

@inject EventEase.Services.EventService EventService

<h1>Upcoming Events</h1>

@* ── SEARCH FILTER ──
   @bind="searchTerm" creates TWO-WAY data binding:
   - When the user types, searchTerm updates
   - If we set searchTerm in code, the input updates too
   @oninput fires on every keystroke (vs @onchange which fires on blur) *@
<input
    type="text"
    placeholder="Search events..."
    @bind="searchTerm"
    @bind:event="oninput"
    class="search-input" />

<p>Showing @FilteredEvents.Count() events</p>

@if (!FilteredEvents.Any())
{
    <p class="empty-state">No events match your search.</p>
}
else
{
    @* For small lists (under ~100 items), a regular @foreach is fine.
       For large lists, use <Virtualize> — it only renders visible rows. *@
    <div class="event-grid">
        <Virtualize Items="FilteredEvents.ToList()" Context="ev">
            <EventCard CurrentEvent="ev" />
        </Virtualize>
    </div>
}

@code {
    private List<Event>? _events;

    // Two-way bound to the search input above
    private string searchTerm = string.Empty;

    // This is a COMPUTED PROPERTY — it recalculates whenever searchTerm changes.
    // IEnumerable<Event> is a lazy sequence (more efficient than List for filtering).
    private IEnumerable<Event> FilteredEvents =>
        _events?.Where(e =>
            string.IsNullOrEmpty(searchTerm) ||
            e.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            e.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        ) ?? Enumerable.Empty<Event>();

    protected override async Task OnInitializedAsync()
    {
        _events = await Task.FromResult(EventService.GetAllEvents());
    }
}
```

---

## Updated Components/EventCard.razor — Error Boundary

> **Error Boundaries** (`<ErrorBoundary>`) catch rendering exceptions in child components
> and show a fallback UI instead of crashing the whole page.
> This is how we handle the case where an Event object has bad/null data.

```razor
@* Components/EventCard.razor — Updated with null safety *@
@using EventEase.Models

@* Wrap the whole card in an ErrorBoundary.
   If anything inside crashes, the <ErrorContent> block shows instead. *@
<ErrorBoundary>
    <ChildContent>
        @* Null check — if CurrentEvent somehow wasn't passed in, show a fallback *@
        @if (CurrentEvent == null)
        {
            <div class="event-card event-card--error">
                <p>Event data unavailable.</p>
            </div>
        }
        else
        {
            <div class="event-card @(CurrentEvent.HasAvailability ? "" : "event-card--full")">
                <h3 class="event-card__title">
                    @* Null-coalescing: if Name is empty, show a fallback string *@
                    @(!string.IsNullOrEmpty(CurrentEvent.Name) ? CurrentEvent.Name : "Unnamed Event")
                </h3>

                <p class="event-card__date">
                    📅 @CurrentEvent.Date.ToString("MMMM dd, yyyy")
                </p>

                <p class="event-card__location">
                    📍 @(!string.IsNullOrEmpty(CurrentEvent.Location) ? CurrentEvent.Location : "Location TBD")
                </p>

                @if (CurrentEvent.HasAvailability)
                {
                    <span class="badge badge--available">
                        @CurrentEvent.RegisteredCount / @CurrentEvent.Capacity spots filled
                    </span>
                }
                else
                {
                    <span class="badge badge--full">FULLY BOOKED</span>
                }

                <a href="/event/@CurrentEvent.Id" class="btn btn--primary">View Details</a>
            </div>
        }
    </ChildContent>

    <ErrorContent Context="ex">
        @* This block renders if an exception is thrown inside ChildContent *@
        <div class="event-card event-card--error">
            <p>⚠️ Failed to display this event.</p>
        </div>
    </ErrorContent>
</ErrorBoundary>

@code {
    [Parameter]
    public Event? CurrentEvent { get; set; }   // Note: now nullable with "?"
}
```

---

---

# 🟢 ACTIVITY 3 — Advanced Features: Registration Form, State & Sessions

---

## Services/SessionService.cs

> **State Management in Blazor Server:**
> Because Blazor Server runs on the server (not in the browser), a service
> registered as `Scoped` lives for the duration of one user's SignalR connection.
> This makes it perfect as a simple "session" store — it holds data between page navigations.
>
> When the user registers, we save their info in SessionService.
> When they visit another page, we can read it back.

```csharp
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
```

---

## Pages/Register.razor

> **EditForm** is Blazor's built-in form component.
> It works with DataAnnotations on your model to validate inputs automatically.
>
> Key concepts:
> - `<EditForm Model="@_registration">` binds the form to our Registration object
> - `<DataAnnotationsValidator />` activates the [Required], [EmailAddress] etc. attributes
> - `<ValidationSummary />` shows all errors at once
> - `<ValidationMessage For="...">` shows the error for one specific field
> - `<InputText @bind-Value="...">` is a two-way bound text input

```razor
@* Pages/Register.razor *@
@page "/register/{EventId:int}"

@using EventEase.Models
@inject EventEase.Services.EventService EventService
@inject EventEase.Services.SessionService SessionService
@inject NavigationManager NavManager

@if (_event == null)
{
    <div class="not-found">
        <h2>Event not found</h2>
        <a href="/" class="btn btn--secondary">← Back</a>
    </div>
}
else if (_registrationSuccess)
{
    @* ── SUCCESS STATE — shown after a valid form submission *@
    <div class="success-card">
        <h2>✅ Registration Confirmed!</h2>
        <p>Thank you, <strong>@SessionService.CurrentUser?.FullName</strong>!</p>
        <p>You're registered for <strong>@_event.Name</strong>.</p>
        <p>A confirmation will be sent to <strong>@SessionService.CurrentUser?.Email</strong>.</p>
        <a href="/" class="btn btn--primary">Browse More Events</a>
    </div>
}
else
{
    <div class="register-page">
        <h1>Register for @_event.Name</h1>
        <p>📅 @_event.Date.ToString("MMMM dd, yyyy") — 📍 @_event.Location</p>

        @* Check if the user already registered for this event (from session) *@
        @if (SessionService.IsRegisteredFor(EventId))
        {
            <div class="alert alert--warning">
                ⚠️ You've already registered for this event in this session.
            </div>
        }

        @* EditForm — Blazor's form handler.
           OnValidSubmit fires only if ALL validation passes. *@
        <EditForm Model="@_registration" OnValidSubmit="HandleValidSubmit">

            @* These two lines activate automatic validation from DataAnnotations *@
            <DataAnnotationsValidator />
            <ValidationSummary />   @* Shows all errors at the top of the form *@

            <div class="form-group">
                <label for="fullName">Full Name</label>
                @* InputText is Blazor's <input> that knows about validation state *@
                <InputText id="fullName" @bind-Value="_registration.FullName" class="form-control" />
                @* ValidationMessage shows the error for just this field *@
                <ValidationMessage For="@(() => _registration.FullName)" />
            </div>

            <div class="form-group">
                <label for="email">Email Address</label>
                <InputText id="email" @bind-Value="_registration.Email" class="form-control" />
                <ValidationMessage For="@(() => _registration.Email)" />
            </div>

            <div class="form-group">
                <label for="phone">Phone Number</label>
                <InputText id="phone" @bind-Value="_registration.Phone" class="form-control" />
                <ValidationMessage For="@(() => _registration.Phone)" />
            </div>

            @* Show error from service (e.g., event became full between page load and submit) *@
            @if (!string.IsNullOrEmpty(_errorMessage))
            {
                <div class="alert alert--error">@_errorMessage</div>
            }

            <button type="submit" class="btn btn--primary" disabled="@_isSubmitting">
                @(_isSubmitting ? "Registering..." : "Confirm Registration")
            </button>
        </EditForm>
    </div>
}

@code {
    [Parameter]
    public int EventId { get; set; }

    private Event? _event;
    private Registration _registration = new();
    private bool _registrationSuccess = false;
    private bool _isSubmitting = false;
    private string _errorMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _event = await Task.FromResult(EventService.GetEventById(EventId));

        // Pre-fill the EventId on the registration model
        _registration.EventId = EventId;
    }

    // This method is called by EditForm only when ALL validation passes
    private async Task HandleValidSubmit()
    {
        // Prevent double-submission (e.g., user clicks twice)
        if (_isSubmitting) return;
        _isSubmitting = true;
        _errorMessage = string.Empty;

        // Simulate async work (e.g., saving to a database)
        await Task.Delay(500);

        // Try to register — the service checks capacity
        bool success = EventService.RegisterForEvent(EventId);

        if (success)
        {
            // Save to session so other pages can see the user is registered
            _registration.EventName = _event?.Name ?? string.Empty;
            SessionService.SetUser(_registration);
            _registrationSuccess = true;
        }
        else
        {
            _errorMessage = "Sorry, this event is now fully booked.";
        }

        _isSubmitting = false;
    }
}
```

---

## Components/AttendanceTracker.razor

> **AttendanceTracker** is a pure display component — it receives data via parameters
> and shows it. No logic of its own. Good example of a "dumb" / presentational component.

```razor
@* Components/AttendanceTracker.razor *@
@using EventEase.Models

<div class="attendance-tracker">
    <h3>Session Attendance</h3>

    @if (!RegisteredEventIds.Any())
    {
        <p class="empty-state">You haven't registered for any events yet.</p>
    }
    else
    {
        <p>You're registered for <strong>@RegisteredEventIds.Count</strong> event(s):</p>
        <ul>
            @foreach (var id in RegisteredEventIds)
            {
                @* Look up the event name from the ID *@
                var ev = AllEvents.FirstOrDefault(e => e.Id == id);
                if (ev != null)
                {
                    <li>✅ @ev.Name</li>
                }
            }
        </ul>
    }
</div>

@code {
    // List of all events (needed to look up names from IDs)
    [Parameter]
    public List<Event> AllEvents { get; set; } = new();

    // IDs the user has registered for (comes from SessionService)
    [Parameter]
    public List<int> RegisteredEventIds { get; set; } = new();
}
```

---

## Updated Pages/EventDetails.razor — Shows Session-Aware State

```razor
@* Pages/EventDetails.razor — Updated to use SessionService *@
@page "/event/{Id:int}"

@using EventEase.Models
@inject EventEase.Services.EventService EventService
@inject EventEase.Services.SessionService SessionService

@if (_event == null)
{
    <div class="not-found">
        <h2>Event not found</h2>
        <a href="/" class="btn btn--secondary">← Back to Events</a>
    </div>
}
else
{
    <div class="event-details">
        <a href="/" class="back-link">← All Events</a>
        <h1>@_event.Name</h1>

        <div class="event-details__meta">
            <span>📅 @_event.Date.ToString("dddd, MMMM dd, yyyy")</span>
            <span>📍 @_event.Location</span>
        </div>

        <p class="event-details__description">@_event.Description</p>

        <div class="event-details__capacity">
            <strong>Capacity:</strong> @_event.RegisteredCount / @_event.Capacity registered
        </div>

        @* Three different states: already registered, full, or available *@
        @if (SessionService.IsRegisteredFor(_event.Id))
        {
            <div class="alert alert--success">✅ You're registered for this event!</div>
        }
        else if (!_event.HasAvailability)
        {
            <button class="btn btn--disabled" disabled>Fully Booked</button>
        }
        else
        {
            <a href="/register/@_event.Id" class="btn btn--primary">Register Now</a>
        }
    </div>
}

@code {
    [Parameter]
    public int Id { get; set; }

    private Event? _event;

    protected override async Task OnInitializedAsync()
    {
        _event = await Task.FromResult(EventService.GetEventById(Id));
    }
}
```

---

## wwwroot/css/app.css — Styling

```css
/* wwwroot/css/app.css */

/* ── CSS Variables — change these to retheme the whole app ── */
:root {
    --primary: #2563eb;
    --primary-hover: #1d4ed8;
    --success: #16a34a;
    --warning: #d97706;
    --error: #dc2626;
    --bg: #f8fafc;
    --surface: #ffffff;
    --border: #e2e8f0;
    --text: #1e293b;
    --text-muted: #64748b;
    --radius: 12px;
    --shadow: 0 1px 3px rgba(0,0,0,0.1), 0 1px 2px rgba(0,0,0,0.06);
}

body {
    font-family: 'Segoe UI', system-ui, sans-serif;
    background: var(--bg);
    color: var(--text);
    margin: 0;
}

/* ── Navbar ── */
.navbar {
    background: var(--surface);
    border-bottom: 1px solid var(--border);
    padding: 1rem 2rem;
    display: flex;
    align-items: center;
    justify-content: space-between;
}
.navbar__brand { font-size: 1.3rem; font-weight: 700; text-decoration: none; color: var(--primary); }
.navbar__links a { margin-left: 1.5rem; color: var(--text); text-decoration: none; }
.navbar__links a.active { color: var(--primary); font-weight: 600; }

/* ── Layout ── */
.main-content { max-width: 1100px; margin: 2rem auto; padding: 0 1.5rem; }
.footer { text-align: center; padding: 2rem; color: var(--text-muted); font-size: 0.875rem; }

/* ── Event Grid ── */
.event-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 1.5rem;
    margin-top: 1.5rem;
}

/* ── Event Card ── */
.event-card {
    background: var(--surface);
    border: 1px solid var(--border);
    border-radius: var(--radius);
    padding: 1.5rem;
    box-shadow: var(--shadow);
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    transition: transform 0.2s, box-shadow 0.2s;
}
.event-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
.event-card--full { opacity: 0.75; }
.event-card--error { border-color: var(--error); }
.event-card__title { margin: 0; font-size: 1.2rem; }
.event-card__date, .event-card__location { margin: 0; color: var(--text-muted); font-size: 0.9rem; }

/* ── Badges ── */
.badge { display: inline-block; padding: 0.25rem 0.75rem; border-radius: 999px; font-size: 0.8rem; font-weight: 600; }
.badge--available { background: #dcfce7; color: #166534; }
.badge--full { background: #fee2e2; color: #991b1b; }

/* ── Buttons ── */
.btn { display: inline-block; padding: 0.6rem 1.4rem; border-radius: 8px; text-decoration: none; font-weight: 600; cursor: pointer; border: none; font-size: 0.95rem; margin-top: 0.5rem; transition: background 0.2s; }
.btn--primary { background: var(--primary); color: white; }
.btn--primary:hover { background: var(--primary-hover); }
.btn--secondary { background: transparent; border: 1px solid var(--border); color: var(--text); }
.btn--disabled { background: var(--border); color: var(--text-muted); cursor: not-allowed; }

/* ── Search ── */
.search-input { width: 100%; max-width: 400px; padding: 0.6rem 1rem; border: 1px solid var(--border); border-radius: 8px; font-size: 1rem; margin-bottom: 0.5rem; }

/* ── Event Details ── */
.event-details { max-width: 700px; }
.event-details__meta { display: flex; gap: 2rem; color: var(--text-muted); margin: 1rem 0; }
.event-details__description { line-height: 1.7; }
.event-details__capacity { margin: 1rem 0; }
.back-link { color: var(--primary); text-decoration: none; font-size: 0.9rem; }

/* ── Registration Form ── */
.register-page { max-width: 500px; }
.form-group { display: flex; flex-direction: column; gap: 0.3rem; margin-bottom: 1.2rem; }
.form-group label { font-weight: 600; font-size: 0.9rem; }
.form-control { padding: 0.6rem 0.8rem; border: 1px solid var(--border); border-radius: 8px; font-size: 1rem; }
.form-control:focus { outline: 2px solid var(--primary); border-color: transparent; }

/* Validation messages (red text under fields) */
.validation-message { color: var(--error); font-size: 0.85rem; margin-top: 0.2rem; }

/* ── Alerts ── */
.alert { padding: 0.75rem 1rem; border-radius: 8px; margin: 1rem 0; }
.alert--success { background: #dcfce7; color: #166534; }
.alert--warning { background: #fef9c3; color: #854d0e; }
.alert--error { background: #fee2e2; color: #991b1b; }

/* ── Success Card ── */
.success-card { background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); padding: 2rem; max-width: 500px; text-align: center; }

/* ── Not Found ── */
.not-found, .not-found-page { text-align: center; padding: 4rem 1rem; }

/* ── Attendance Tracker ── */
.attendance-tracker { background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); padding: 1.5rem; margin-top: 2rem; }
.attendance-tracker ul { list-style: none; padding: 0; }
.attendance-tracker li { padding: 0.4rem 0; }

/* ── Empty state ── */
.empty-state { color: var(--text-muted); font-style: italic; }
```

---

---

# 📋 Key Blazor Concepts — Quick Reference

| Concept | Syntax | What it does |
|---|---|---|
| Page route | `@page "/path"` | Gives a component a URL |
| Route parameter | `@page "/event/{Id:int}"` | Captures URL segments |
| Inject service | `@inject ServiceType Name` | DI: gets a service instance |
| One-way binding | `@someValue` | Renders a value into HTML |
| Two-way binding | `@bind="fieldName"` | Syncs input ↔ variable |
| Conditional render | `@if (condition) { }` | Renders block if true |
| Loop render | `@foreach (var x in list) { }` | Renders one block per item |
| Component param | `[Parameter] public T Prop { get; set; }` | Accepts value from parent |
| Lifecycle hook | `OnInitializedAsync()` | Runs when component loads |
| Form | `<EditForm Model="@obj" OnValidSubmit="Handler">` | Validated form |
| Validation | `<DataAnnotationsValidator />` | Activates model attributes |
| Error boundary | `<ErrorBoundary>` | Catches render exceptions |
| Virtualization | `<Virtualize Items="list">` | Efficient large-list rendering |

---

# 🚀 How to Run

1. Open the project in **Visual Studio 2022**
2. Press **F5** (or the green Play button)
3. The app opens at `https://localhost:xxxx`
4. Navigate: Events List → View Details → Register

---

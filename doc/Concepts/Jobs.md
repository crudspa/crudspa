# Concepts | Jobs

Background work has a habit of drifting outside the main application architecture. A few scripts appear. Then a scheduler lives somewhere else. Then the UI can no longer tell you what ran, why it failed, or how it was configured.

Crudspa keeps jobs inside the same architectural world as the rest of the platform. Jobs are modeled as data. Schedules are modeled as data. Job types are modeled as data plus typed configuration and executable actions.

In this repository, that story is deliberately split across normal web hosts and a dedicated engine host. The web hosts expose `Jobs` and `Schedules` panes, while the engine host runs the scheduler and worker loops. The split is practical: users manage jobs in the same UI they already use, while scheduling and execution still run in a focused background process.

## Core Job Model

Three records sit at the center of the jobs system:

* a `job`, which represents one queued or running unit of work
* a `job schedule`, which defines when work should be created
* a `job type`, which tells the system which editor view and action class should be used

That model gives the platform a place to store:

* status
* description
* schedule linkage
* device targeting
* next-run timing
* type-specific configuration

This is what keeps jobs visible instead of mysterious.

That split is what makes the jobs system operationally visible. Job schedules explain why work appears, job types explain how it runs and how it's edited, and job records explain what actually happened.

## Admin Surfaces

Crudspa includes client-side administration surfaces for job records and schedules, and it allows job types to supply type-specific editor UIs. That's important because many jobs need structured configuration, not just a name and a cron string.

Those admin surfaces can live in the same host UI where the rest of the application work already happens. There doesn't need to be a separate jobs website just to create, inspect, and manage background work.

This is also where the plugin model shows up again. A job type can point to its own editor surface, and the worker side can point to its own executable action class.

The result is a jobs system that remains operationally visible without flattening every job type into the same generic settings form.

## Execution Roles

The server-side execution model is typically split into two long-running roles inside the engine host.

In this repository's engine host, both roles create their own framework sessions at startup. Those sessions begin anonymous and still use the normal portal and device configuration, which keeps local engine setup simple without inventing a separate jobs-only identity model.

### Scheduler

The scheduler checks for due schedules, creates job records, and computes the next run time for each schedule.

In this repository's engine host, it also reschedules overdue work on startup so the local database doesn't get stuck with stale `Next Run` values after a debugging session or an unclean shutdown.

### Worker

The worker fetches runnable job batches, instantiates the action class for each job type, configures the action from its stored JSON, runs it, and saves the resulting status.

This worker flow is intentionally dynamic. The framework owns the infrastructure around sessions, status persistence, batching, and notices. The job type owns the actual business action.

On startup, the worker can also mark previously `Running` jobs for its device as `Canceled` before it begins polling again. That gives developers a clean recovery path when they stop and restart the engine during debugging.

## Cross-Host Feedback

The interesting part isn't just that the engine runs jobs. It's how the rest of the platform stays in sync with that work.

The web hosts create and edit jobs through the normal hub and service boundary. The engine then reads and writes the same database records, but it doesn't talk back to the UI through a private side channel. Instead, it publishes typed gateway events such as `JobAdded`, `JobSaved`, `JobStatusChanged`, and `JobScheduleSaved`.

Through configured receiver URLs or other gateway targets, those events can be relayed into other hosts and turned back into normal SignalR notices. That lets jobs panes refresh through the same notice model the rest of the platform already uses, including live status transitions while work is starting, finishing, or being canceled.

This is why the jobs system still feels like Crudspa instead of a bolt-on task runner. UI work, engine work, and real-time feedback stay in the same architectural vocabulary.

## Extending With App-Specific Actions

Framework behavior ends at the action boundary. Project-specific work begins when you implement a new job action and define the configuration it needs.

In practice, adding a new job type usually means:

* defining a typed config contract
* adding an editor surface for that config when needed
* implementing an `IJobAction`
* registering whatever services the action depends on
* making sure the action reports success or failure clearly

Try to keep job actions focused and safe to rerun. Background work is easier to operate when each action is explicit about what it reads, what it writes, and how it reports failure.

## Tradeoffs

Crudspa's jobs model is more structured than handing work to a script folder or an external task runner. That structure is intentional. It keeps jobs observable, configurable, and aligned with the rest of the platform.

The tradeoff is that even a simple operational task may deserve contracts, UI, status tracking, and an engine host. In production systems, that's usually a strength rather than overhead.

## Next Steps

* [Overview | Libraries](../Overview/Libraries.md)
* [Overview | Architecture](../Overview/Architecture.md)
* [Concepts | Services](Services.md)
* [Documentation Index](../ReadMe.md)

# Patterns | Fill

`Fill` is the submission-first pattern. Use it when the job is intake: collect input, validate it, save it once, and reset quickly for the next submission.

It stays intentionally narrow. A good fill surface avoids record-maintenance concerns and focuses on one pass through one add flow.

## Flow

1. Initialize blank or default entity state.
2. Submit one add-style request through the normal `Request<T>` and `Response<T>` boundary.
3. Let the hub apply the required session or permission wrapper.
4. Let the service validate, normalize, sanitize, and insert.
5. Show success feedback and reset for the next submission.

That last step is what separates `Fill` from `Edit`. A fill workflow is optimized for repeated entry, not for reopening and maintaining one long-lived record.

## Public Repo Status

The current public sample track doesn't center a dedicated fill pane. That makes this page more of a pattern guide than a code tour today.

Even so, the surrounding pieces already exist in public code:

* `SignInEmailTfaModel` shows the narrow, submission-first style Crudspa prefers for short auth actions such as password reset and access-code entry.
* `TrackServiceSql.Add(...)` shows the server-side shape Crudspa expects for add flows: validate first, sanitize where needed, then insert through SQL.
* `TrackServiceHub.TrackAdd(...)` shows that even simple add flows still cross the normal wrapper and notice boundary.

That combination is enough to define the standard shape clearly, even without a dedicated fill-pane sample host.

## What Belongs In The Pattern

`Fill` is usually the right fit when all of the following are true:

* the user is creating one new record at a time
* the post-submit experience should return to a blank or ready state
* the workflow doesn't need browse, reopen, delete, or relation-management behavior
* speed of repeated entry is more important than long-lived record maintenance

Typical examples include signup, request, feedback, intake, and other front-door submission workflows.

## Pressure Points

### Data Hygiene

Fill surfaces often accept free text from unknown or semi-trusted users. Sanitization, normalization, and validation need to happen in service and SQL boundaries, not only in the browser.

### Duplicate And Replay Risks

If submissions can be repeated accidentally by double-clicks, refreshes, reconnects, or back-button behavior, add idempotency rules server-side. Do not rely only on button disabling.

### Follow-Up Work

Many fill flows trigger downstream processing such as email, jobs, or back-office review. Keep those side effects in service logic, not in pane code.

### Pattern Drift

When a fill surface starts needing reopen, edit, delete, relation management, or history views, it has crossed into [Patterns | Edit](Edit.md) territory.

## Tradeoffs

`Fill` maximizes speed and clarity for intake workflows, but it's intentionally narrow. It gives you a fast submission loop, not a full record-maintenance experience.

## Next Steps

* [Patterns | Edit](Edit.md)
* [Patterns | Find](Find.md)
* [Concepts | Services](../Concepts/Services.md)
* [Documentation Index](../ReadMe.md)

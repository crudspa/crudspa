# Concepts | Forums

Crudspa separates forum authoring from forum participation. Authors configure forums in `Content.Design`; runtime applications use `Content.Display` to list forums, search threads, read nested comments, post, edit, remove, react, and work with attached media.

The `Composer` and `Consumer` samples make that boundary concrete. `Composer` owns configuration. `Consumer` owns the public reading and signed-in participation experience.

## Authoring Model

A forum belongs to a portal and carries the policy needed by the runtime host:

* title, description, image, status, and display order
* an access mode of `Everyone` or `LicensedUsers`
* optional permission and license relationships
* optional tag bundles used to classify threads

The authoring host also maintains threads and their opening posts. Runtime comments and reactions remain part of the display-side experience.

## Runtime Contract

[`IForumRunService`](../../src/Content/Display/Shared/Contracts/Behavior/IForumRunService.cs) is the shared runtime boundary. It covers:

* listing accessible forums and fetching one forum
* searching and fetching threads
* adding, editing, and removing threads
* fetching, adding, editing, and removing nested comments
* setting reactions

The client proxy, server hub, SQL service, and stored procedures follow the same contract pattern used elsewhere in Crudspa. A host opts in by registering the runtime service on both client and server.

## Access And Participation

Forum access is enforced on every read and write, not inferred from whether a pane is visible.

An `Everyone` forum can be read anonymously when it belongs to the current portal and does not require an additional permission. A `LicensedUsers` forum is returned only when the session resolves to one of the configured licenses. Hosts supply that license lookup through `ISessionLicenseResolver`.

Reading and participation are intentionally different capabilities. The public sample allows anonymous reading, while thread, comment, reaction, and media mutations require a signed-in session. Edit and delete flags are computed for the current user and checked again on the server before a write succeeds.

## Content Safety And Limits

The runtime service sanitizes comment HTML before saving it and applies the shared limits in [`ForumPolicy`](../../src/Content/Display/Shared/Contracts/Data/ForumPolicy.cs). The current policy caps message bodies, comments per thread, and nested reply depth.

Media uses a staged upload flow. Access is checked when a file is staged, consumed, or fetched, and failed or expired uploads can be discarded. That keeps forum attachments inside the same session, license, and data-enforcement boundary as the discussion itself.

## Sample Walkthrough

1. Run `Composer` and `Consumer` together.
2. In `Composer`, open the seeded `Consumer` portal and choose `Forums`.
3. Inspect the sample forums, threads, opening posts, access modes, and tag configuration.
4. In `Consumer`, browse the same forums without signing in.
5. Sign in with `sample@example.com` by using the local reset-password flow, then add a thread or nested comment and try a reaction.

## Practical Guidance

When adding forums to another host:

* register `IForumRunService` in the client and server compositions
* register `ISessionLicenseResolver` on the server, even if the host intentionally resolves no licenses
* expose the forum pane types through portal metadata
* keep anonymous reads and authenticated mutations explicit in both UI behavior and server enforcement
* route forum media through the supplied controller and service instead of exposing blob paths directly

## Next Steps

* [Concepts | Security](Security.md)
* [Concepts | Licensing](Licensing.md)
* [Concepts | Services](Services.md)
* [Overview | Samples](../Overview/Samples.md)
* [Documentation Index](../ReadMe.md)

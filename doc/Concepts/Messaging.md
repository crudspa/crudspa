# Concepts | Messaging

`Content.Messaging` turns reusable audience definitions and message templates into staged campaigns. It follows the same client, shared, server, and SQL boundaries as the rest of Crudspa, while leaving application-specific audience lookup and delivery providers behind explicit interfaces.

The `Composer` sample exposes the authoring surfaces. `Content.Jobs` supplies the population-refresh and email or SMS delivery actions that a jobs engine can execute.

## Core Model

The messaging model is deliberately layered:

| Record | Responsibility |
| --- | --- |
| `Population` | names a portal-scoped audience and selects the resolver responsible for producing members |
| `Campaign` | groups a licensed communication sequence for one portal |
| `Stage` | positions a send relative to campaign, lesson, or assessment start and defines send time and weekend adjustment |
| `Message` | assigns one email or SMS template and one population to a stage |
| `Activation` | applies a campaign to an organization and start date so its messages can be scheduled |

A campaign is a reusable definition. An activation is a concrete run of that definition.

## Scheduling

Stages are ordered and calculate delivery from an anchor date plus a day offset. The shared schedule calculator supports campaign, lesson, and assessment anchors, and can keep a calculated date exact or move a weekend send to the previous or next weekday.

Send times are interpreted in the target time zone. Invalid local times during daylight-saving transitions advance to the next valid minute before conversion to UTC.

## Population Extension Point

`IPopulationResolver` is the main audience extension point. Each resolver has a unique key, advertises its supported tokens, and resolves a population for an organization and optional activation scope.

The sample Composer registers two resolvers:

* `OrganizationUsersPopulationResolver` for portal users in an organization
* `StaticMembershipPopulationResolver` for a fixed membership list

Applications can register their own resolvers without changing campaign services. Duplicate resolver keys fail during registry construction so configuration mistakes surface early.

## Activation Targets

`IActivationTargetProvider` supplies the organizations that can receive a campaign. Providers are selected by portal ID and can search and validate targets. This keeps product-specific organization rules out of the reusable messaging module.

## Channels And Delivery

A message chooses exactly one email or SMS template. Email and SMS services retain membership-level delivery records, while channel and sender interfaces keep operational providers replaceable.

The Composer sample demonstrates local email and SMS output. Email is written under `C:\data\temp\email`, and SMS is written under `C:\data\temp\sms`, so contributors can exercise delivery flows without sending real messages.

`Content.Jobs` contains the actions that refresh populations and send queued email or SMS work. Run the sample jobs engine beside `Composer` when you want records to move through scheduling and delivery rather than only studying the authoring panes.

## Sample Walkthrough

1. Run `Composer` and publish the current sample database.
2. Sign in with `sample@example.com` by using the local reset-password flow.
3. Under the seeded `Consumer` portal, open `Campaigns`.
4. Inspect the sample population, campaign, ordered stages, and email or SMS message definitions.
5. Run `Samples/Jobs/Engine` when you want to study population refresh or delivery processing.
6. Inspect the local email and SMS folders instead of expecting an external provider to send messages.

## Practical Guidance

When composing messaging in another host:

* register the campaign, stage, message, population, and activation services in client and server projects
* register at least one `IPopulationResolver` for every resolver key stored by the host
* register an `IActivationTargetProvider` for each portal that supports activation targeting
* choose email and SMS sender implementations explicitly in server composition
* run the content job actions in a jobs engine when scheduled delivery is required
* keep provider credentials and production endpoints in deployment configuration, never in tracked source

## Next Steps

* [Concepts | Jobs](Jobs.md)
* [Concepts | Injection](Injection.md)
* [Concepts | Licensing](Licensing.md)
* [Overview | Samples](../Overview/Samples.md)
* [Documentation Index](../ReadMe.md)

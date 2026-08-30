# Concepts | Rostering

Rostering is the controlled synchronization of organizations, schools, people, roles, courses, sections, and enrollments from an outside source into a local application. It is not sign-in, and it should not be coupled to sign-in simply because the same vendor offers both services.

Crudspa's `Education.Rostering` module keeps that boundary explicit. A provider retrieves data and writes a staged run. The platform validates it, identifies matches and differences, records issues, and applies an approved run through a local mapping layer. Provider adapters do not write application tables directly.

## The Run Lifecycle

1. A configured `IRosterProvider` starts a run for one roster source.
2. The provider writes neutral staged records through `IRosterSink`.
3. Validation checks structure, limits, mappings, and match candidates.
4. Reviewers inspect blocking issues and proposed changes.
5. The application applies an approved run, records the result, and retains the source mappings needed for the next run.

The source, staged records, issues, and run status are durable data. That makes a repeat run, a review decision, and an authoritative-source cutover explainable later.

## Sources And Authority

An organization can retain more than one source for comparison or migration. Only one source should be authoritative for a given scope at a time. Moving authority is a reviewed operational action, not an incidental consequence of connecting a new provider.

The architecture supports vendor adapters such as Clever, ClassLink, OneRoster, or a solution-specific file feed. Each adapter translates source data into the same neutral stage model. The application pipeline stays the same regardless of how the provider authenticates, paginates, or represents its records.

## Authentication Is Separate

Rostering establishes local people and relationships. Authentication establishes a verified identity and a local session. An external identity must be linked deliberately to a local user; a roster record by itself does not grant sign-in access. Likewise, an authentication policy does not make a provider authoritative for rostering.

This separation lets a team use one vendor for roster data and another for sign-in, change one without silently changing the other, and protect the existing permission model throughout.

## Practical Guidance

* stage first; do not let provider adapters write application records directly
* treat validation errors and reconciliation candidates as reviewable data
* keep external IDs and source mappings stable across runs
* keep shadow and authoritative sources distinct
* require a reviewed cutover before changing authority
* keep raw roster exports and provider credentials outside source control

## Next Steps

* [Components | Authentication](../Components/Authentication.md)
* [Concepts | Security](Security.md)
* [Overview | Architecture](../Overview/Architecture.md)
* [Overview | Samples](../Overview/Samples.md)

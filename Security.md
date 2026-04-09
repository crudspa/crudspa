# Security Policy

Crudspa is used to build data-heavy applications with authentication, authorization, real-time messaging, file handling, and SQL-backed services. Security bugs in any of those layers matter.

## Report Privately

If you discover a vulnerability, email `support@crudspa.org`.

Please do not report suspected vulnerabilities in Discussions, pull requests, or any other public thread.

## What to Include

The fastest way to help us triage a report is to include:

* the affected project, component, endpoint, hub, service, or SQL object
* clear reproduction steps
* the likely impact and attack scenario
* the branch, commit, or version you tested
* logs, screenshots, proof-of-concept code, or a suggested mitigation if you have them

## Response Process

We are a small team, but we do review good-faith security reports.

* We aim to acknowledge reports within 5 business days.
* We will triage severity and confirm whether the issue is reproducible.
* If the report is valid, we will work on a fix in the current source line and coordinate disclosure timing with you.

Crudspa is source-first today, so fixes may land in repository source before there is any packaged release channel for them.

We do not currently offer a bug bounty program.

## Good-Faith Research

We support good-faith security research conducted against systems you own or are explicitly authorized to test.

Please:

* use test accounts and sample data whenever possible
* stop after you have demonstrated the issue
* avoid privacy violations, destructive actions, and service disruption
* keep details private until a fix or mitigation is available

## Scope and Boundaries

This policy applies to source code in this repository.

Please do not:

* access or attempt to access real customer, student, educator, or school data
* run denial-of-service or load tests against production systems
* use social engineering, phishing, or physical attacks
* report unrelated third-party issues unless you can clearly show Crudspa-specific impact

For production service incidents tied to hosted environments, contact `support@crudspa.org` directly.

## Supported Fixes

Crudspa does not currently publish multiple maintained release lines. We investigate reports against current source and fix forward.

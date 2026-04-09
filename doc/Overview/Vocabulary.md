# Overview | Vocabulary

Crudspa uses a small set of terms very deliberately. Learning them early pays off because the same vocabulary carries through the docs, the source tree, the database model, and the runtime shell.

This is not meant to be a giant glossary. It is the short list of terms that unlock the rest of the platform.

## Core Terms

### Solution Terms

| Term | Meaning |
| --- | --- |
| `area` | A top-level source family inside a solution, such as `Framework`, `Content`, or `Education` |
| `module` | A cohesive slice inside an area that groups related client, shared, server, and sometimes database behavior |
| `node` | A feature boundary around one root entity and the contracts, panes, queries, and mutations that belong to it |
| `predicate` | An explicit query constraint used to enforce scope, tenancy, or relationship filtering |
| `sibling` | An additional entity intentionally maintained alongside a node's root entity |

### Shell Terms

| Term | Meaning |
| --- | --- |
| `portal` | The top-level application shell, including branding, session behavior, authentication rules, and navigation metadata |
| `segment` | A navigable branch inside a portal; segments can contain child segments and panes |
| `pane` | A single work surface inside the shell, usually backed by one feature or report |
| `path` | The URL-resolved address to the current shell state; Crudspa treats it as part of application state, not just a route string |
| `report` | A read-oriented, plugin-driven surface rendered inside the shell |

### Content Terms

| Term | Meaning |
| --- | --- |
| `binder` | An ordered content container that delivers a reading, learning, or guided runtime flow across pages |
| `page` | One content unit inside a binder or other content surface, such as a footer page |
| `section` | A layout block inside a page that contains ordered content elements |
| `element` | A typed content block rendered and often edited through a plugin contract |
| `rule` | A typed styling instruction whose configuration shapes how authored content is presented |

## Related Groups

Several of these terms make the most sense as families.

* A `portal` contains `segments`.
* A `segment` can contain child `segments` and `panes`.
* A `pane` often fronts a `node` or a `report`.
* A `binder` contains `pages`.
* A `page` contains `sections`.
* A `section` contains ordered `elements`.
* A `rule` helps determine how those elements ultimately look at runtime.

The key idea is that Crudspa tries to keep structure visible. Navigation structure, content structure, and service structure each have names, and those names are reused consistently.

## How To Read The Rest Of The Docs

If a page uses one of these terms heavily, it should use it in this sense.

When you are reading about shell behavior, keep `portal`, `segment`, `pane`, and `path` in mind. When you are reading about CRUD patterns, keep `node`, `predicate`, and `sibling` in mind. When you are reading about authored experiences, keep `binder`, `page`, `section`, `element`, and `rule` in mind.

You do not need to memorize the whole list up front. Just return here whenever a term feels more specific than everyday English.

## Next Steps

* [Overview | Architecture](Architecture.md)
* [Concepts | Navigation](../Concepts/Navigation.md)
* [Concepts | Plugins](../Concepts/Plugins.md)
* [Documentation Index](../ReadMe.md)

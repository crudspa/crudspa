# Concepts | Tenancy

Tenancy is where many business applications stop being simple. One application scopes everything to an organization. Another scopes by district, publisher, provider, school, or some relationship chain that only makes sense in that domain.

Crudspa's approach is practical: derive scope from the current session whenever possible, describe the rule clearly, and enforce the final predicate close to the data.

## Crudspa's Stance

Crudspa thinks about tenancy as a question:

*Given this session, which rows should this request be allowed to read or change?*

That question is more useful than arguing about one universal tenancy model, because real applications rarely have one.

## Why This Shows Up Publicly

This isn't just a database detail. It affects how teams build with the framework.

If tenancy is treated as a first-class concern:

* service calls stay easier to reason about
* shared repositories can remain reusable
* navigation can respect scope as well as permissions
* applications become safer to evolve when the domain gets more complex

## A Typical Crudspa Pattern

Most tenancy-sensitive work follows the same broad shape:

1. The application sends the current `SessionId`.
2. The boundary confirms the session and any needed permission.
3. The service coordinates the feature-level work.
4. The final SQL rule derives scope from that session and applies the real predicate.

That last step is the important one. Crudspa wants the hard guarantee to survive at the data layer.

## Different Applications, Different Predicates

Some applications are organization-scoped. Others are district-scoped or provider-scoped. Crudspa doesn't try to flatten those into one fake universal rule.

That flexibility helps because line-of-business software often grows through exactly those differences.

## Repositories And Tenancy

Crudspa repositories are usually shared persistence helpers, not the final authority on feature scope. That's why a service can reuse shared contact or user persistence logic while a feature-specific write still applies the true tenancy rule for the feature itself.

That separation keeps common persistence code reusable without weakening the final rule.

## Practical Guidance

When adding a tenancy-sensitive feature:

* write the rule down in plain language first
* derive effective scope from the session, not only from caller-supplied IDs
* enforce the final rule where the shared data is actually touched
* accept that different applications may need different predicates

## Tradeoffs

Crudspa's tenancy model asks teams to take scope seriously all the way down to the data layer. That's more deliberate than treating tenancy as a browser convention, but it's much safer once the application has real complexity.

## Next Steps

* [Concepts | Security](Security.md)
* [Concepts | Repositories](Repositories.md)
* [Databases | Access](../Databases/Access.md)
* [Documentation Index](../ReadMe.md)

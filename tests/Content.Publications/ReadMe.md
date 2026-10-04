# Publication integration checks

Run `dotnet run --project tests/Content.Publications` after deploying the shared SQL project and publication catalogs to `Crudspa-Local`.

These checks exercise real SQL-backed authoring and public services: atomic child batches, draft/retired visibility, site and organization isolation, resource destinations, group deletion, and ordering. They use the existing Composer, Consumer, and Catalog sample identities, create temporary sessions and fixture records, and remove only those fixture rows in `finally`. File uploads are verified separately in the local browser walkthrough.

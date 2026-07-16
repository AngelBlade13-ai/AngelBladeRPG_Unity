# Asset Ownership And License Tracking

Every externally created or contributed asset must have a clearance record
before entering a public build. Keep receipts, licenses, source URLs, signed
agreements, and attribution text in a private administrative archive; do not
commit personal addresses, signatures, tax data, or confidential contracts to
the public repository.

## Status Values

- `Proposed`: not yet imported or approved.
- `Pending`: present for internal evaluation but not cleared for release.
- `Cleared`: rights and evidence reviewed for the intended commercial use.
- `Rejected`: must not be used or distributed.
- `Retired`: previously cleared but intentionally removed from production.

Only `Cleared` assets may ship publicly.

## Asset Log

Add one row per asset or clearly defined asset pack.

| Asset ID | Project path/use | Asset type | Creator/source | Ownership or license | Commercial use | Modification | Redistribution restrictions | Attribution | AI disclosure | Evidence archive path | Status | Reviewer/date |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `example_placeholder` | `Assets/...` | Sprite | `[SOURCE]` | `[OWNED/LICENSE]` | `[YES/NO]` | `[RULE]` | `[RULE]` | `[TEXT/NONE]` | `[DETAILS/NONE]` | `[PRIVATE PATH]` | `Proposed` | `[NAME/DATE]` |

Delete the example row after the first real entry is recorded.

## Contributor Agreement Log

| Contributor ID | Public credit | Role/scope | Agreement date/version | Compensation option | Rights evidence archive path | Deliverables covered | Status |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `[ID]` | `[NAME]` | `[ROLE]` | `[DATE/VERSION]` | `[FLAT/SHARE/DEFERRED/UNPAID]` | `[PRIVATE PATH]` | `[ASSET IDS]` | `Pending` |

## Required Review

For every asset, confirm:

- The creator and initial owner are identified.
- Commercial video-game use, marketing use, modification, and distribution
  inside game builds are permitted.
- Source/editor files are covered when required.
- Store capsules, trailers, soundtrack releases, merchandise, ports, and
  localization are covered or explicitly excluded.
- Attribution wording and placement are recorded.
- Share-alike, copyleft, noncommercial, editorial-only, seat, project-count,
  platform, territory, and expiration restrictions are understood.
- Fonts cover embedding and redistribution in builds.
- Music covers composition, recording, performance, samples, and loops.
- Generative-AI provenance and service terms are disclosed and approved.
- A receipt or signed agreement alone is not treated as the license unless it
  actually grants the required rights.

## Repository Workflow

1. Assign a stable asset ID before production use.
2. Record the source and intended use.
3. Store legal evidence in the private archive and reference it by path or ID.
4. Keep uncleared files out of release-addressable asset groups and builds.
5. Review the log before screenshots, trailers, external playtests, store review,
   demo release, and full release.
6. Record replacements and retirement rather than deleting audit history.

Friend-specific agreements remain `Pending` until role, scope, compensation,
credit, and rights are signed. Informal permission is not release clearance.


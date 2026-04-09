---
id: api
slug: /api
title: API Reference
---

InfiniFrame uses a split API docs strategy during the DocFX to Docusaurus migration:

- Conceptual API guidance lives in Docusaurus:
  - [Native C++ API Guide](cpp/native-cpp-api.md)
- Generated API references remain hosted separately on the legacy DocFX endpoint:
  - [Generated C# API Reference](https://docs.infiniframe.dev/api/cs/)
  - [Generated C++ API Reference](https://docs.infiniframe.dev/api/cpp/native-cpp-reference.html)

This keeps generated API output stable while Markdown guides and concepts move to Docusaurus.

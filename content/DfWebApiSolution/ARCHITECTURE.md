# Architecture and assembly boundaries

This document is the source of truth for where code belongs and for keeping
project dependencies pointed inward as the service grows.

## Dependency direction

```text
Models <- Abstraction
             ^
      BusinessLogic
        ^    ^    ^
      DAL  HttpManager  Validator
         \    |    /
        WebApiTemplate
               ^
    WebApiTemplate.UnitTests
```

`WebApiTemplate` is the composition root. Implementations are connected to
interfaces there through dependency injection.

## Assembly responsibilities

### Models.dll

Contains shared domain models, request and response DTOs, value objects, enums,
and strongly typed configuration models. It must not reference another solution
project and must not contain database or HTTP client code.

### Abstraction.dll

Contains interfaces and ports used to invert dependencies between the business
layer and infrastructure. Repository, outbound API, clock, and token-service
contracts belong here. It may reference `Models` but contains no implementations.

### BusinessLogic.dll

Contains use cases, business rules, application orchestration, and reusable
security logic such as JWT creation. It may depend only on `Abstraction` and
`Models`. It must not reference the Web, DAL, or HttpManager projects.

### DAL.dll

Contains persistence implementations, database commands, repository
implementations, mappings, and transaction behavior. It implements interfaces
from `Abstraction` and uses types from `Models`. Business decisions do not
belong here.

### HttpManager.dll

Contains outbound HTTP integrations, typed clients, serialization of external
contracts, resilience policies, and remote error translation. Incoming API
controllers do not belong here.

### Validator.dll

Contains structural and input validation that can run before a use case. Rules
that require business state or express a business decision belong in
`BusinessLogic`, not here.

### WebApiTemplate.dll

Contains the HTTP transport and composition root: `Program`, `Startup`,
controllers, middleware, dependency registration, authentication configuration,
Swagger, metrics, and health endpoints. Controllers should translate HTTP input
and delegate work rather than implement business rules.

### WebApiTemplate.UnitTests.dll

Contains isolated unit tests, mocks, and JWT behavior tests. Add integration
tests here only while the suite is small; split them into a dedicated project
once they require real infrastructure.

## Non-negotiable rules

1. Models references no solution project.
2. Abstraction references Models only.
3. BusinessLogic references Abstraction and Models only.
4. DAL, HttpManager, and Validator never reference BusinessLogic or Web.
5. Web is the only production project allowed to reference every layer.
6. Secrets and tokens are never written to logs or committed to source control.
7. JWT claims are consumed only after signature, issuer, audience, and lifetime
   validation by the authentication middleware or `IJwtTokenService`.

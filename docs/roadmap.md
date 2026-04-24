# Roadmap

## PR1

- rename `Semcosm.HardwareConsole.app` to `Semcosm.HardwareConsole.App`
- move source projects under `src/`
- split shared models into `Abstractions`
- split mock data providers into `Mock`

## Next

- build `Profiles` with the same `Model + Service + ViewModel + Binding` pattern
- introduce service interfaces where mock and real providers should share a contract
- move reusable cards and presentation primitives into `Controls` and `Styles`
- define plugin manifest loading and discovery flow
- add tests after the abstraction seams settle

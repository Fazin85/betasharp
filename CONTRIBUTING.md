# Contributing to BetaSharp

## Getting Started

For detailed instructions on how to build and run the project, please refer to the Building section in the README file.

## Code Conventions

### 1. Follow .editorconfig

The project includes an `.editorconfig` file. Your IDE (Visual Studio, Rider, VS Code) should automatically respect these settings. **Always follow them.**

### 2. Standards

This is **not** a Java project, please follow well known C# conventions to write idiomatic C# contributions, for more visit Microsoft's reference for C#.

- https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
- https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names

### 3. Remove IKVM/Java Types

- **Goal**: The ultimate goal of this project is to **remove ALL usage of IKVM and Java types**.
- **New Code**: Must **exclusively** use C# types, collections, and I/O. Do not introduce new Java dependencies.
- **Refactoring**: When cleaning up existing code, prioritize converting IKVM/Java types to their C# equivalents.
- **Exceptions**: If converting to C# would break significant logic or requires a massive rewrite that blocks progress, the IKVM/Java code may remain *temporarily*. However, it should be marked for future refactoring.

### 5. AI Policy

- **Allowed**: AI tools (ChatGPT, Copilot, etc.) are allowed to assist with coding and refactoring.
- **Quality Control**: Low-quality, "vibe-coded", or hallucinated code will be **rejected**.
- **Review**: You are responsible for every line of code you submit. Verify that AI-generated code is correct, follows project conventions, and compiles before submitting.

## Workflow

1. **Fork** the repository.
2. Create a **Feature Branch** for your changes (`git checkout -b feature/my-cool-feature`).
3. **Commit** your changes (`git commit -m "Add some cool feature"`).
4. **Push** to your branch (`git push origin feature/my-cool-feature`).
5. Open a **Pull Request**.

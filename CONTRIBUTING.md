# Contributing to BetaSharp

## Getting Started

For detailed instructions on how to build and run the project, please refer to the [Building section in README.md](README.md#building).

## Code Conventions

### 1. Follow .editorconfig
The project includes an `.editorconfig` file. Your IDE (Visual Studio, Rider, VS Code) should automatically respect these settings. **Always follow them.**

### 2. C# Standards
- Use **PascalCase** for classes, methods, properties, and events.
- Use **camelCase** for local variables and method parameters.
- Use standard C# collection types (`List<T>`, `Dictionary<K,V>`, `Queue<T>`, etc.) instead of Java ports.

### 3. Remove IKVM / Java Types
- **Goal**: The ultimate goal of this project is to **remove ALL usage of IKVM and Java types**.
- **New Code**: Must **exclusively** use C# types, collections, and I/O. Do not introduce new Java dependencies.
- **Refactoring**: When cleaning up existing code, prioritize converting Java/IKVM types to their C# equivalents.
- **Exceptions**: If converting to C# would break significant logic or requires a massive rewrite that blocks progress, the Java/IKVM code may remain *temporarily*. However, it should be marked for future refactoring.

### 4. General Style
- **Do not use Java/IKVM style.**
    - Avoid `snake_case` or `camelCase` for methods (e.g., use `GetBlock()` instead of `getBlock()`).
    - Use C# Properties instead of Get/Set methods where appropriate.

### 5. AI Policy
- **Allowed**: AI tools (ChatGPT, Copilot, etc.) are allowed to assist with coding and refactoring.
- **Quality Control**: Low-quality, "vibe-coded", or hallucinated code will be **rejected**.
- **Review**: You are responsible for every line of code you submit. Verify that AI-generated code is correct, follows project conventions, and compiles before submitting.

## Workflow

1.  **Fork** the repository.
2.  Create a **Feature Branch** for your changes (`git checkout -b feature/my-cool-feature`).
3.  **Commit** your changes (`git commit -m "Add some cool feature"`).
4.  **Push** to your branch (`git push origin feature/my-cool-feature`).
5.  Open a **Pull Request**.

## Reporting Issues

### Bug Reports
When reporting a bug, you **MUST** include:
1.  **Machine Specifications**:
    - OS (Windows/Linux/macOS version)
    - CPU
    - GPU
    - RAM
2.  **Reproduction Steps**: Detailed steps to reproduce the issue.
3.  **Expected vs. Actual Behavior**: What happened vs. what you expected to happen.
4.  **Logs/Screenshots**: Any relevant error logs or screenshots.

### Feature Requests
- Provide a clear and concise description of the feature you would like to see.

## Pull Requests
- Keep PRs focused on a single feature or fix.
- Link any related issues in the description.
- Ensure your code builds and runs without errors before submitting.

Thank you for your contributions!

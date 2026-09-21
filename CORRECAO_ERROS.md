# Correção dos erros de compilação

## Erro 1 — NETSDK1022 / CS0111 / CS0121: Ficheiros C# duplicados

O SDK .NET inclui automaticamente todos os ficheiros `.cs` da pasta do projeto.
Se tiveres os ficheiros `App.xaml.cs`, `MainWindow.xaml.cs`, etc. listados
**explicitamente** no `.csproj`, ficam incluídos duas vezes.

### Solução: abre o ficheiro `.csproj` e remove as linhas de `<Compile Include="..."/>`

O teu `.csproj` deve ficar assim (sem entradas `<Compile>` manuais):

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <!-- Assets das cartas como Resource -->
  <ItemGroup>
    <Resource Include="Assets\**\*.*"/>
  </ItemGroup>

</Project>
```

A linha `<Resource Include="Assets\**\*.*"/>` também resolve o erro XDG0005
(card_back.png não encontrado).

---

## Erro 2 — XDG0005: Cannot locate resource 'assets/card_back.png'

Este erro acontece porque:
1. O ficheiro não está marcado como **Resource** no projeto (resolve com o .csproj acima), OU
2. A capitalização da pasta não bate certo.

### Verifica:
- A pasta chama-se exatamente `Assets` (com A maiúsculo)?
- O ficheiro chama-se exatamente `card_back.png` (tudo minúsculas)?

Se a pasta se chamar `assets` (minúsculo), muda todos os caminhos no código
de `/Assets/...` para `/assets/...`.

---

## Resumo dos passos

1. Abre o ficheiro `Jogo.csproj`
2. Remove todas as linhas `<Compile Include="..."/>` que existam
3. Adiciona `<Resource Include="Assets\**\*.*"/>` dentro de um `<ItemGroup>`
4. Confirma que a pasta `Assets` existe na raiz do projeto com os ficheiros dentro
5. Faz **Build → Clean Solution** e depois **Build → Build Solution**


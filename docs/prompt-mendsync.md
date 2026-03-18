# Prompt: Criação da Aplicação MendSync — .NET 8 + Clean Architecture

## Contexto

Você é um desenvolvedor sênior .NET. Sua tarefa é criar do zero uma aplicação chamada **MendSync**, uma API e camada de serviço que consome a **Mend API 3.0** para facilitar o gerenciamento de vulnerabilidades, projetos, findings e usuários — substituindo a navegação lenta e travada do portal Mend.

**Stack obrigatória:**
- .NET 8 (ASP.NET Core Web API)
- Clean Architecture: Controller → Service → HttpClient (Mend API)
- Nenhum banco de dados neste momento — tudo via chamadas à API do Mend
- Autenticação JWT própria (gerada pela Mend API) com refresh automático
- HttpClient tipado com Polly para retry
- FluentValidation para inputs
- Serilog para logging
- Swagger/OpenAPI para documentação

---

## Estrutura de Pastas

```
MendSync/
├── src/
│   ├── MendSync.API/                   # Projeto ASP.NET Core (entry point)
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── ApplicationsController.cs
│   │   │   ├── ProjectsController.cs
│   │   │   ├── FindingsController.cs
│   │   │   ├── ScansController.cs
│   │   │   ├── ReportsController.cs
│   │   │   ├── UsersController.cs
│   │   │   ├── GroupsController.cs
│   │   │   └── LabelsController.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionMiddleware.cs
│   │   │   └── TokenRefreshMiddleware.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── MendSync.Application/           # Interfaces + DTOs + Use Cases
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IApplicationService.cs
│   │   │   ├── IProjectService.cs
│   │   │   ├── IFindingsService.cs
│   │   │   ├── IScansService.cs
│   │   │   ├── IReportsService.cs
│   │   │   ├── IUsersService.cs
│   │   │   ├── IGroupsService.cs
│   │   │   └── ILabelsService.cs
│   │   └── DTOs/
│   │       ├── Auth/
│   │       ├── Applications/
│   │       ├── Projects/
│   │       ├── Findings/
│   │       ├── Scans/
│   │       ├── Reports/
│   │       ├── Users/
│   │       ├── Groups/
│   │       └── Labels/
│   │
│   └── MendSync.Infrastructure/        # Implementações dos serviços
│       ├── Services/
│       │   ├── AuthService.cs
│       │   ├── ApplicationService.cs
│       │   ├── ProjectService.cs
│       │   ├── FindingsService.cs
│       │   ├── ScansService.cs
│       │   ├── ReportsService.cs
│       │   ├── UsersService.cs
│       │   ├── GroupsService.cs
│       │   └── LabelsService.cs
│       ├── HttpClients/
│       │   └── MendApiClient.cs        # HttpClient base tipado
│       └── Token/
│           └── TokenStore.cs           # In-memory store do JWT + refresh token
```

---

## Configuração Base (appsettings.json)

```json
{
  "Mend": {
    "BaseUrl": "https://api-app.mend.io",
    "Email": "",
    "UserKey": "",
    "OrgUuid": ""
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

---

## Autenticação Mend API 3.0

A Mend API usa um fluxo de 2 etapas:

**Etapa 1 — Login (obtém refreshToken):**
```
POST /api/v3.0/login
Body: { "email": "user@empresa.com", "userKey": "abc123..." }
Response: { "refreshToken": "...", "userUuid": "..." }
```

**Etapa 2 — Gerar Access Token JWT (expira em ~10 min):**
```
POST /api/v3.0/login/accessToken
Header: Authorization: Bearer {refreshToken}
Body: {} (vazio ou com accessToken para renovar)
Response: { "jwtToken": "...", "orgUuid": "...", "tokenTTL": 600000, ... }
```

**Todas as chamadas subsequentes usam:**
```
Authorization: Bearer {jwtToken}
```

**Regras de token:**
- O `jwtToken` expira a cada ~10 minutos
- Deve haver renovação automática transparente
- O `refreshToken` dura a sessão (até o logout)
- Logout: `POST /api/v3.0/logout`

---

## Implementação do TokenStore e Auto-Refresh

```csharp
// MendSync.Infrastructure/Token/TokenStore.cs
public class TokenStore
{
    private string? _refreshToken;
    private string? _jwtToken;
    private DateTime _jwtExpiry = DateTime.MinValue;

    public void SetRefreshToken(string token) => _refreshToken = token;
    public string? GetRefreshToken() => _refreshToken;

    public void SetJwtToken(string token, long ttlMs)
    {
        _jwtToken = token;
        _jwtExpiry = DateTime.UtcNow.AddMilliseconds(ttlMs - 30000); // 30s buffer
    }

    public bool IsJwtValid() => _jwtToken != null && DateTime.UtcNow < _jwtExpiry;
    public string? GetJwtToken() => _jwtToken;
    public void Clear() { _refreshToken = null; _jwtToken = null; _jwtExpiry = DateTime.MinValue; }
}
```

---

## MendApiClient — HttpClient Base Tipado

```csharp
// MendSync.Infrastructure/HttpClients/MendApiClient.cs
public class MendApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenStore _tokenStore;
    private readonly IAuthService _authService;

    public MendApiClient(HttpClient httpClient, TokenStore tokenStore, IAuthService authService)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
        _authService = authService;
    }

    private async Task EnsureTokenAsync()
    {
        if (!_tokenStore.IsJwtValid())
            await _authService.RefreshAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenStore.GetJwtToken());
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.GetAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.PostAsJsonAsync(endpoint, body, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.PutAsJsonAsync(endpoint, body, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    public async Task DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.DeleteAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
        {
            Content = JsonContent.Create(body)
        };
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }
}
```

---

## Módulos e Endpoints a Implementar

Implemente **todos os grupos abaixo**, seguindo o padrão: Controller chama Interface → Implementação chama MendApiClient.

---

### 1. Auth (Access Management)

**Interface: IAuthService**
```csharp
Task<LoginResponseDto> LoginAsync(string email, string userKey);
Task<AccessTokenResponseDto> RefreshAccessTokenAsync();
Task LogoutAsync();
```

**Endpoints Mend:**
| Método | Mend Endpoint | Descrição |
|--------|--------------|-----------|
| POST | `/api/v3.0/login` | Login — obtém refreshToken |
| POST | `/api/v3.0/login/accessToken` | Gera/renova JWT |
| POST | `/api/v3.0/logout` | Logout — revoga refreshToken |

**Controller: AuthController**
- `POST /api/auth/login` — recebe email + userKey, faz login completo (etapa 1 + etapa 2), retorna JWT ao caller
- `POST /api/auth/logout`

---

### 2. Applications

**Interface: IApplicationService**
```csharp
Task<PagedResultDto<ApplicationDto>> GetApplicationsAsync(string orgUuid, PaginationParams pagination);
Task<IEnumerable<ApplicationSummaryDto>> GetApplicationStatisticsAsync(string orgUuid, ApplicationStatsRequestDto request);
Task<ApplicationTotalsDto> GetApplicationTotalsAsync(string orgUuid);
Task<IEnumerable<LabelDto>> GetApplicationLabelsAsync(string orgUuid, string applicationUuid);
Task AddApplicationLabelAsync(string orgUuid, string applicationUuid, string labelUuid);
Task RemoveApplicationLabelAsync(string orgUuid, string applicationUuid, string labelUuid);
Task<IEnumerable<ScanDto>> GetApplicationScansAsync(string orgUuid, string applicationUuid);
Task UpdateApplicationViolationSlaAsync(string orgUuid, string applicationUuid, UpdateViolationSlaDto request);
```

**Endpoints Mend:**
| Método | Mend Endpoint |
|--------|--------------|
| GET | `/api/v3.0/orgs/{orgUuid}/applications` |
| POST | `/api/v3.0/orgs/{orgUuid}/applications/summaries` |
| GET | `/api/v3.0/orgs/{orgUuid}/applications/summaries/totals` |
| GET | `/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/labels` |
| PUT | `/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/labels` |
| DELETE | `/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/labels/{labelUuid}` |
| GET | `/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/scans` |
| PUT | `/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/violations/sla` |

---

### 3. Projects

**Interface: IProjectService**
```csharp
Task<PagedResultDto<ProjectDto>> GetProjectsAsync(string orgUuid, PaginationParams pagination);
Task<IEnumerable<ProjectSummaryDto>> GetProjectStatisticsAsync(string orgUuid, ProjectStatsRequestDto request);
Task<ProjectTotalsDto> GetProjectTotalsAsync(string orgUuid);
Task<ProjectTotalsByDateDto> GetProjectTotalsByDateAsync(string orgUuid);
Task<IEnumerable<LabelDto>> GetProjectLabelsAsync(string orgUuid, string projectUuid);
Task AddProjectLabelAsync(string orgUuid, string projectUuid, string labelUuid);
Task RemoveProjectLabelAsync(string orgUuid, string projectUuid, string labelUuid);
Task<IEnumerable<ViolationDto>> GetProjectViolationsAsync(string orgUuid, string projectUuid);
Task UpdateProjectViolationSlaAsync(string orgUuid, string projectUuid, UpdateViolationSlaDto request);
Task<IEnumerable<EffectiveVulnerabilityDto>> GetProjectEffectiveVulnerabilitiesAsync(string projectUuid);
```

**Endpoints Mend:**
| Método | Mend Endpoint |
|--------|--------------|
| GET | `/api/v3.0/orgs/{orgUuid}/projects` |
| POST | `/api/v3.0/orgs/{orgUuid}/projects/summaries` |
| GET | `/api/v3.0/orgs/{orgUuid}/projects/summaries/total/date` |
| GET | `/api/v3.0/orgs/{orgUuid}/projects/summaries/totals` |
| GET | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/labels` |
| PUT | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/labels` |
| DELETE | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/labels/{labelUuid}` |
| GET | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/violations` |
| PUT | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/violations/sla` |
| GET | `/api/v3.0/projects/{projectUuid}/dependencies/effective` |

---

### 4. Findings (Project Level)

**Interface: IFindingsService**
```csharp
// Dependencies (SCA)
Task<PagedResultDto<SecurityFindingDto>> GetProjectSecurityFindingsAsync(string projectUuid, FindingsFilterParams filters);
Task<IEnumerable<RootLibraryFindingDto>> GetRootLibraryFindingsAsync(string projectUuid);
Task UpdateRootLibraryFindingAsync(string projectUuid, string rootLibraryUuid, UpdateFindingDto request);
Task<IEnumerable<LibraryDto>> GetProjectLibrariesAsync(string projectUuid);
Task<IEnumerable<LibraryLicenseDto>> GetProjectLicensesAsync(string projectUuid);

// SAST (Code)
Task<PagedResultDto<CodeFindingDto>> GetProjectCodeFindingsAsync(string projectUuid, CodeFindingsFilterParams filters);
Task BulkUpdateCodeFindingsAsync(string projectUuid, BulkUpdateCodeFindingsDto request);
Task UpdateCodeFindingAsync(string projectUuid, string findingSnapshotId, UpdateCodeFindingDto request);
Task<CodeFindingDetailDto> GetCodeFindingDetailAsync(string projectUuid, string findingUuid);

// Containers (Images)
Task<IEnumerable<ImageSecurityFindingDto>> GetImageSecurityFindingsAsync(string projectUuid);
Task UpdateMultipleImageFindingsAsync(string projectUuid, UpdateMultipleImageFindingsDto request);
Task UpdateImageFindingAsync(string projectUuid, string findingId, UpdateImageFindingDto request);
Task<IEnumerable<SecretFindingDto>> GetImageSecretFindingsAsync(string projectUuid);
Task<IEnumerable<ImagePackageDto>> GetImagePackagesAsync(string projectUuid);

// AI
Task<IEnumerable<AiTechnologyDto>> GetAiTechnologiesAsync(string projectUuid);
Task<IEnumerable<AiModelDto>> GetAiModelsAsync(string projectUuid);
Task<IEnumerable<AiVulnerabilityDto>> GetAiVulnerabilitiesAsync(string projectUuid);
Task<AiVulnerabilityDetailDto> GetAiVulnerabilityDetailAsync(string projectUuid, string vulnerabilityId);
```

---

### 5. Scans

**Interface: IScansService**
```csharp
Task<PagedResultDto<ScanDto>> GetProjectScansAsync(string orgUuid, string projectUuid, PaginationParams pagination);
Task<ScanDetailDto> GetScanAsync(string orgUuid, string projectUuid, string scanUuid);
Task<ScanSummaryDto> GetScanSummaryAsync(string orgUuid, string projectUuid, string scanUuid);
Task<IEnumerable<ScanTagDto>> GetScanTagsAsync(string orgUuid, string projectUuid, string scanUuid);
```

**Endpoints Mend:**
| Método | Mend Endpoint |
|--------|--------------|
| GET | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/scans` |
| GET | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/scans/{scanUuid}` |
| GET | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/scans/{scanUuid}/summary` |
| GET | `/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/scans/{scanUuid}/tags` |

---

### 6. Reports

**Interface: IReportsService**
```csharp
Task<ReportStatusDto> GetReportStatusAsync(string orgUuid, string reportUuid);
Task<IEnumerable<ReportDto>> GetReportsAsync(string orgUuid);
Task<byte[]> DownloadReportAsync(string orgUuid, string reportUuid);
Task DeleteReportAsync(string orgUuid, string reportUuid);

// Geração assíncrona por escopo (Project / Application / Org)
Task<ReportProcessDto> ExportDependencySecurityFindingsAsync(string scope, string scopeUuid, ReportExportRequestDto request);
Task<ReportProcessDto> ExportSbomAsync(string scope, string scopeUuid, ReportExportRequestDto request);
Task<ReportProcessDto> ExportDueDiligenceAsync(string scope, string scopeUuid, ReportExportRequestDto request);
Task<ReportProcessDto> ExportSastFindingsAsync(string scope, string scopeUuid, ReportExportRequestDto request);
Task<ReportProcessDto> ExportSastComplianceAsync(string scope, string scopeUuid, ReportExportRequestDto request);
```

**Nota importante:** Reports são assíncronos. O fluxo é: (1) disparar geração → recebe `reportUuid`, (2) polling de status até `COMPLETED`, (3) download.

Implemente um método helper `WaitForReportAsync(orgUuid, reportUuid)` que faz polling com 5s de intervalo e timeout de 5 minutos.

---

### 7. Users (Administration)

**Interface: IUsersService**
```csharp
Task<PagedResultDto<UserDto>> GetUsersAsync(string orgUuid, PaginationParams pagination);
Task InviteUserAsync(string orgUuid, InviteUserDto request);
Task RemoveUserAsync(string orgUuid, string userUuid);
Task BlockUserAsync(string orgUuid, string userUuid);
Task UnblockUserAsync(string orgUuid, string userUuid);
```

**Endpoints Mend:**
| Método | Mend Endpoint |
|--------|--------------|
| GET | `/api/v3.0/orgs/{orgUuid}/users` |
| POST | `/api/v3.0/orgs/{orgUuid}/users/invite` |
| DELETE | `/api/v3.0/orgs/{orgUuid}/users/{userUuid}` |
| PUT | `/api/v3.0/orgs/{orgUuid}/users/{userUuid}/block` |
| PUT | `/api/v3.0/orgs/{orgUuid}/users/{userUuid}/unblock` |

---

### 8. Groups (Administration)

**Interface: IGroupsService**
```csharp
Task<IEnumerable<GroupDto>> GetGroupsAsync(string orgUuid);
Task<GroupDto> CreateGroupAsync(string orgUuid, CreateGroupDto request);
Task<GroupDto> GetGroupAsync(string orgUuid, string groupUuid);
Task UpdateGroupAsync(string orgUuid, string groupUuid, UpdateGroupDto request);
Task DeleteGroupAsync(string orgUuid, string groupUuid);
Task<IEnumerable<RoleDto>> GetGroupRolesAsync(string orgUuid, string groupUuid);
Task AddGroupRolesAsync(string orgUuid, string groupUuid, GroupRolesDto request);
Task RemoveGroupRolesAsync(string orgUuid, string groupUuid, GroupRolesDto request);
Task<IEnumerable<UserDto>> GetGroupUsersAsync(string orgUuid, string groupUuid);
Task AddUserToGroupAsync(string orgUuid, string groupUuid, string userUuid);
Task RemoveUserFromGroupAsync(string orgUuid, string groupUuid, string userUuid);
```

---

### 9. Labels (Administration)

**Interface: ILabelsService**
```csharp
Task<IEnumerable<LabelDto>> GetLabelsAsync(string orgUuid);
Task<LabelDto> AddLabelAsync(string orgUuid, CreateLabelDto request);
Task RenameLabelAsync(string orgUuid, string labelUuid, RenameLabelDto request);
Task RemoveLabelAsync(string orgUuid, string labelUuid);
```

---

## Paginação Padrão (Cursor-based — Mend API 3.0)

A Mend API usa cursor pagination com os parâmetros:
```
pageSize (int, default 25, max 200)
cursor (string, opcional — valor retornado pela página anterior)
```

Crie um DTO genérico:
```csharp
public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Total { get; set; }
    public string? NextCursor { get; set; }
    public bool HasMore => NextCursor != null;
}

public class PaginationParams
{
    public int PageSize { get; set; } = 25;
    public string? Cursor { get; set; }
}
```

---

## Registro de Dependências (Program.cs)

```csharp
builder.Services.AddSingleton<TokenStore>();

builder.Services.AddHttpClient<MendApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Mend:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(GetRetryPolicy());

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IFindingsService, FindingsService>();
builder.Services.AddScoped<IScansService, ScansService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IGroupsService, GroupsService>();
builder.Services.AddScoped<ILabelsService, LabelsService>();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
```

---

## Tratamento de Erros

Crie um `ExceptionMiddleware` global que:
- Captura `HttpRequestException` com status 401 → tenta refresh do token uma vez, reenvia
- Captura `HttpRequestException` com status 403 → retorna 403 com mensagem clara
- Captura `HttpRequestException` com status 404 → retorna 404
- Captura `HttpRequestException` com status 429 → retorna 429 com header `Retry-After`
- Captura exceções genéricas → loga com Serilog, retorna 500

**Envelope de resposta padrão:**
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string? SupportToken { get; set; } // do campo Mend
}
```

---

## Requisitos Adicionais

1. **OrgUuid automático:** Após login bem-sucedido, o `orgUuid` retornado pela Mend deve ser armazenado no `TokenStore` e usado automaticamente em todos os endpoints que precisam dele — eliminar necessidade de passar orgUuid em cada chamada do cliente da API.

2. **Swagger:** Configurar com autenticação Bearer JWT. Adicionar exemplos de request/response nos controllers.

3. **Health Check:** Endpoint `GET /health` que chama a Mend API e retorna status da conectividade.

4. **Logging:** Logar todas as chamadas à Mend API (endpoint, status, tempo de resposta) via Serilog.

5. **Configuração via User Secrets (dev) e Environment Variables (prod):** Nunca expor `userKey` no código.

---

## Ordem de Implementação Sugerida

1. Estrutura de projetos (.sln + 3 projetos)
2. TokenStore + MendApiClient
3. AuthService + AuthController (testa o fluxo de login completo)
4. ProjectService + ProjectController (módulo mais usado)
5. FindingsService + FindingsController (core do produto)
6. ApplicationService + ApplicationController
7. ScansService + ScansController
8. ReportsService + ReportsController (com polling)
9. UsersService + GroupsService + LabelsService (administração)
10. Middleware de exceção + Logging + Swagger
11. Health Check

---

## Objetivo Final

O MendSync deve ser uma alternativa fluida ao portal Mend, expondo uma API bem estruturada que pode ser consumida por um front-end Angular (a ser criado em seguida), permitindo:
- Visualizar e filtrar projetos, findings e vulnerabilidades com muito mais performance
- Gerenciar usuários, grupos e labels com facilidade
- Gerar e baixar relatórios de forma programática
- Escalar para receber persistência em banco de dados futuramente sem refatoração da camada de domínio

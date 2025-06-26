📌 Project Overview – Koperasi System
This system implements a modular user registration and authentication platform, designed to be extensible, secure, and localized. The registration flow includes:

Personal info submission

OTP verification (mobile/email)

Policy agreement

PIN setup

Biometric preference

It supports migrating users, localization, and uses rate limiting and encryption/salting for security.

🏗️ Architecture
✅ Layered (Onion) Architecture:
API Layer: Entry point. Receives HTTP requests and returns HTTP responses.

Application Layer: Business logic layer (e.g., services like RegistrationService).

Core Layer: Interfaces (IUserService, IUnitOfWork), DTOs, and enums.

Infrastructure Layer: Implements database logic, security helpers, context extensions, UoW, and generic repository.

🧠 Design Patterns Used
Pattern	Description
Repository Pattern	Abstracts DB logic, e.g., _userRepository, FindAsync, etc.
Unit of Work (UoW)	Manages transactions via IUnitOfWork, ensuring consistency.
Service Layer	All registration/login logic resides in RegistrationService.
Factory / Helper	Helpers like HashingHelpers for salting and hashing.
Strategy/Enum-Based Flow	Enum-driven RegistrationStatus used for process flow control.

🧰 Tools & Technologies
Tool	Purpose
ASP.NET Core	Web API backend
Entity Framework Core	ORM, migrations, seedings
Identity Framework	User management
IMemoryCache	Multilingual localization dictionary cache
Swagger	Auto-generated API docs
Twilio	OTP SMS sending
SMTP	Email OTP delivery
AspNetCoreRateLimit	Global API rate limiting
PostgreSQL / SQL Server	Underlying DB (assumed)

📁 Folder Structure
plaintext
Copy
Edit
├── Core
│   ├── Services            # Interfaces (e.g., IUserService)
│   ├── UnitOfWork          # IUnitOfWork interface
│   └── Enums               # All enums like RegistrationStatusEnum
│
├── Application
│   └── Services            # Logic for registration, login, migration
│
├── Infrastructure
│   ├── Data                # EF Core configuration, seeding
│   ├── Extensions          # Cache, context helpers
│   ├── Helpers             # Security, HTTP context, etc.
│   ├── Repositories        # Generic and concrete repo logic
│   └── Config              # EntityTypeConfiguration classes
│
├── Models
│   ├── Entities            # DB models (e.g., Users, DictionaryLocalization)
│   ├── ViewModels          # DTOs for request and response
│   └── Enums               # Shared enums
│
├── Koperasi.API
│   ├── Controllers         # Entry points (e.g., AccountController)
│   ├── appsettings.json    # Configuration, rate limits, Twilio/SMTP
│   └── Program.cs          # Middleware, Swagger, RateLimit, etc.
🔄 Registration Execution Path (From API to DB)
POST /account/personal-info

Controller → RegistrationService.CreateUserAsync

User created → OTP generated → Twilio SMS sent

POST /account/mobile-verification

Controller → VerifyMobileOTPAsync

OTP checked → Status updated → Email OTP generated

POST /account/email-verification

Controller → VerifyEmailOTPAsync

OTP validated → Status updated to PolicyApproval

POST /account/policy-approval

Status advanced to PINSetup

POST /account/pin-setup

PIN hashed and saved → Status: BiometricSetup

POST /account/biometric-setup

Biometric preference saved → Status: Completed

🔁 Migration Flow (Partial Users)
POST /account/login

If user exists but is not Status: Completed, flow re-triggers from where left off.

🌍 Localization
Dictionary entries cached via IMemoryCache

Localized messages accessed via _memoryCache.GetWord(languageId, dictionaryId)

Example: error 178 = "Invalid OTP"

🔐 Security Measures
OTP and PIN use salted SHA256 hash

Biometric flag is stored, but not the biometric data

Rate limiting middleware prevents abuse:

json
Copy
Edit
"GeneralRules": [
  { "Endpoint": "*", "Period": "1m", "Limit": 10 }
]
🧪 Swagger Testing & Developer Access
Swagger UI available at /swagger

Each endpoint:

Shows request & response model clearly

Annotated with [ProducesResponseType(typeof(...))]

Developers can test endpoints directly and view sample inputs


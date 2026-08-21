# License Key Auth System

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13-239120?style=flat-square&logo=csharp)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux-blue?style=flat-square)
![Build](https://img.shields.io/badge/Build-Passing-brightgreen?style=flat-square)

> **HWID Lock + Subscriptions + Anti-Crack | Client + Server**

A complete license key authentication system with hardware ID binding, subscription management, anti-tampering protection, and a REST API server for key management.

---

## Features

### Client Library
- **HWID Generation** — Unique hardware fingerprint from CPU, motherboard, and disk serials
- **License Validation** — RSA signature verification for tamper-proof license files
- **Session Management** — Heartbeat-based session tracking with auto-expiry
- **Integrity Checking** — Debugger detection, module scanning, memory CRC verification
- **Request Signing** — HMAC-SHA256 signed API requests to prevent replay attacks

### Server
- **RESTful API** — Clean endpoints for authentication, heartbeat, and administration
- **Admin Panel** — Generate, revoke, and manage license keys
- **HWID Binding** — First-use hardware lock with admin reset capability
- **Rate Limiting** — Per-IP request throttling to prevent brute-force attacks
- **Tier System** — Support for multiple subscription levels (Basic, Pro, Lifetime)

---

## Architecture

```
src/
├── KeyAuth.Client/          # Client library
│   ├── Config/              # Configuration models
│   ├── Crypto/              # Request signing
│   ├── Models/              # Data models
│   └── Utils/               # HTTP helpers
└── KeyAuth.Server/          # REST API server
    ├── Controllers/         # API endpoints
    ├── Config/              # Server configuration
    ├── Data/                # Data access layer
    ├── Middleware/          # Rate limiting
    ├── Models/              # Shared models
    └── Services/            # Business logic
```

---

## Build Instructions

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Build
```bash
dotnet build License-Key-Auth-System-CSharp.slnx
```

### Run Server
```bash
cd src/KeyAuth.Server
dotnet run
```

### Run Client (Example)
```bash
cd src/KeyAuth.Client
dotnet run -- --key "KEY-XXXXXX-XXXXXX-XXXXXX-XXXXXX" --server "http://localhost:5000"
```

---

## Configuration

### Client (`authconfig.json`)
```json
{
  "ServerUrl": "https://auth.example.com",
  "ApplicationId": "my-app",
  "AppSecret": "your-secret-here",
  "AppVersion": "1.0.0",
  "VerifyIntegrity": true
}
```

### Server (`appsettings.json`)
```json
{
  "Server": {
    "DatabasePath": "keys.db",
    "Port": 5000,
    "AdminToken": "your-admin-token",
    "MaxRequestsPerMinute": 60,
    "RequireHttps": true
  }
}
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/validate` | Validate license key + HWID |
| POST | `/api/auth/heartbeat` | Session keepalive |
| POST | `/api/admin/generate` | Generate new license key |
| POST | `/api/admin/revoke` | Revoke existing key |
| POST | `/api/admin/reset-hwid` | Reset HWID binding |
| GET | `/api/admin/stats` | Server statistics |

---

## Disclaimer

This project is provided for **educational and research purposes only**. It demonstrates common patterns used in software licensing systems including hardware fingerprinting, cryptographic validation, and client-server authentication flows. Use responsibly and in accordance with applicable laws. The authors assume no liability for misuse of this software.

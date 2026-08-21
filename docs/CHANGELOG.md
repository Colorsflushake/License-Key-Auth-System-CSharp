# Changelog

All notable changes to this project will be documented in this file.

## [1.2.0] - 2026-07-15

### Added
- HWID reset endpoint for admin panel
- Rate limiter middleware with configurable thresholds
- Session heartbeat mechanism

### Changed
- Improved HWID generation algorithm to include disk serial
- Upgraded to .NET 9

## [1.1.0] - 2026-06-01

### Added
- Admin controller with key generation and revocation
- Integrity checker with debugger detection
- Request signing with HMAC-SHA256

### Fixed
- Race condition in session cleanup timer

## [1.0.0] - 2026-05-10

### Added
- Initial release
- Client library with HWID-based authentication
- Server with license validation
- RSA signature verification for license files

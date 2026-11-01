# Authentication

Browser MVC uses the named `PoshakGhor.Cookie` scheme with an HttpOnly, always-secure, SameSite=Lax cookie and sliding expiration. API clients use the separate named `PoshakGhor.Bearer` JWT scheme. Future API authorization policies/controllers must select the bearer scheme explicitly; the current default scheme is the cookie scheme for browser navigation.

Set `Authentication__Jwt__Issuer`, `Authentication__Jwt__Audience`, `Authentication__Jwt__SigningKey`, and `Authentication__Jwt__ExpiryMinutes` through environment variables or development user secrets. Issuer, audience, signing key, and lifetime validation are enabled. The empty configuration values are placeholders; no token issuer, login, registration, OTP, refresh flow, or identity storage exists. Configure a strong secret before enabling protected API routes.

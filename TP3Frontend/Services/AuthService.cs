using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using TP3Frontend.Models;

namespace TP3Frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public AuthResponse? UsuarioActual { get; private set; }

        public event Action? SesionCambiada;

        public bool EstaAutenticado =>
            UsuarioActual != null &&
            !string.IsNullOrWhiteSpace(UsuarioActual.Token) &&
            !TokenVencido(UsuarioActual.Token);

        public string Rol =>
            UsuarioActual?.Rol ?? string.Empty;

        public bool EsAdmin =>
            Rol == "Admin";

        public bool EsVendedor =>
            Rol == "Vendedor";

        public bool TieneRol(params string[] roles)
        {
            return EstaAutenticado &&
                   roles.Contains(Rol);
        }


        public AuthService(
            HttpClient http,
            IJSRuntime js)
        {
            _http = http;
            _js = js;
        }


        public async Task<bool> LoginAsync(
            string username,
            string password)
        {
            var login = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response = await _http.PostAsJsonAsync(
                "api/Auth/Login",
                login
            );

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var auth =
                await response.Content
                    .ReadFromJsonAsync<AuthResponse>();

            if (auth == null ||
                string.IsNullOrWhiteSpace(auth.Token))
            {
                return false;
            }

            UsuarioActual = auth;

            await _js.InvokeVoidAsync(
                "localStorage.setItem",
                "token",
                auth.Token
            );

            await _js.InvokeVoidAsync(
                "localStorage.setItem",
                "nombre",
                auth.Nombre
            );

            await _js.InvokeVoidAsync(
                "localStorage.setItem",
                "username",
                auth.Username
            );

            await _js.InvokeVoidAsync(
                "localStorage.setItem",
                "rol",
                auth.Rol
            );

            await _js.InvokeVoidAsync(
                "localStorage.setItem",
                "usuarioId",
                auth.Id.ToString()
            );

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    auth.Token
                );

            SesionCambiada?.Invoke();

            return true;
        }


        public async Task CargarSesionAsync()
        {
            var token =
                await _js.InvokeAsync<string?>(
                    "localStorage.getItem",
                    "token"
                );

            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            // Si el JWT ya venció, limpiamos la sesión.
            if (TokenVencido(token))
            {
                await LogoutAsync();
                return;
            }

            var nombre =
                await _js.InvokeAsync<string?>(
                    "localStorage.getItem",
                    "nombre"
                );

            var username =
                await _js.InvokeAsync<string?>(
                    "localStorage.getItem",
                    "username"
                );

            var rol =
                await _js.InvokeAsync<string?>(
                    "localStorage.getItem",
                    "rol"
                );

            var usuarioIdTexto =
                await _js.InvokeAsync<string?>(
                    "localStorage.getItem",
                    "usuarioId"
                );

            int.TryParse(
                usuarioIdTexto,
                out var usuarioId
            );

            UsuarioActual = new AuthResponse
            {
                Id = usuarioId,
                Nombre = nombre ?? string.Empty,
                Username = username ?? string.Empty,
                Rol = rol ?? string.Empty,
                Token = token
            };

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token
                );

            SesionCambiada?.Invoke();
        }


        public async Task LogoutAsync()
        {
            UsuarioActual = null;

            _http.DefaultRequestHeaders.Authorization = null;

            await _js.InvokeVoidAsync(
                "localStorage.removeItem",
                "token"
            );

            await _js.InvokeVoidAsync(
                "localStorage.removeItem",
                "nombre"
            );

            await _js.InvokeVoidAsync(
                "localStorage.removeItem",
                "username"
            );

            await _js.InvokeVoidAsync(
                "localStorage.removeItem",
                "rol"
            );

            await _js.InvokeVoidAsync(
                "localStorage.removeItem",
                "usuarioId"
            );

            SesionCambiada?.Invoke();
        }


        private static bool TokenVencido(string token)
        {
            try
            {
                var partes = token.Split('.');

                if (partes.Length < 2)
                {
                    return true;
                }

                var payload = partes[1]
                    .Replace('-', '+')
                    .Replace('_', '/');

                switch (payload.Length % 4)
                {
                    case 2:
                        payload += "==";
                        break;

                    case 3:
                        payload += "=";
                        break;
                }

                var bytes =
                    Convert.FromBase64String(payload);

                using var documento =
                    JsonDocument.Parse(bytes);

                if (!documento.RootElement
                    .TryGetProperty("exp", out var exp))
                {
                    return true;
                }

                var fechaExpiracion =
                    DateTimeOffset.FromUnixTimeSeconds(
                        exp.GetInt64()
                    );

                return fechaExpiracion <=
                       DateTimeOffset.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
}
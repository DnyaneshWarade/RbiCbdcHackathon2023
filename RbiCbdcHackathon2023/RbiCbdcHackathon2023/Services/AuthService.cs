using RbiCbdcHackathon2023.Interfaces;

namespace RbiCbdcHackathon2023.Services
{
    internal class AuthService : IAuthService
    {
        public async Task<bool> AuthenticateUser(string mobileNo, string pin)
        {
            await Task.Delay(2000);
            return false;
        }
    }
}

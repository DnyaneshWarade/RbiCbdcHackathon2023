namespace RbiCbdcHackathon2023.Interfaces
{
    internal interface IAuthService
    {
        Task<bool> AuthenticateUser(string mobileNo, string pin);
    }
}

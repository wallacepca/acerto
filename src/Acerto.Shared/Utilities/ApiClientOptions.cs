namespace Acerto.Shared.Utilities
{
    public abstract class ApiClientOptions
    {
        public string BaseAddress { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

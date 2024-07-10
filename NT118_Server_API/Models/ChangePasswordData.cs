namespace NT118_Server_API.Models
{
    public class ChangePasswordData
    {
        private string _username;
        private string _newPassword;
        private string? _oldPassword;
        public ChangePasswordData()
        {

        }
        public string Username { get => _username; set => _username = value; }
        public string NewPassword { get => _newPassword; set => _newPassword = value; }
        public string? OldPassword { get => _oldPassword; set => _oldPassword = value; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace Yuhnevich_vb_lab.Data
{
    public class AppUser:IdentityUser
    {
        public byte[]? Avatar { get; set; }
        public string? MimeType { get; set; }

    }
}

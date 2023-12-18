using System.ComponentModel.DataAnnotations;

namespace NewsBlog.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public RoleType RoleName { get; set; } = RoleType.User;
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }

    public enum RoleType
    {
        User, 
        Admin 
    }
}

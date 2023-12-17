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
        User, // будет сохранено в базе данных как 0
        Admin // будет сохранено как 1
    }
}

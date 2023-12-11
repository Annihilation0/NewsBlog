using System.ComponentModel.DataAnnotations;

namespace NewsBlog.NewsBlogData
{
    public class Role
    {
        public int RoleId { get; set; }
        [Key]
        public string RoleName { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}

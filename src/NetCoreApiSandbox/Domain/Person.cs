namespace NetCoreApiSandbox.Domain
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public User User { get; set; }
    }
}

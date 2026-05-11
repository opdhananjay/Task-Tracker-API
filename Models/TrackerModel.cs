namespace devops.Models
{
    public class TrackerModel
    {
    }

    public class CreateTask
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string TaskType { get; set; }

        public int? DeveloperId { get; set; }

        public int? TesterId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? DueDateTime { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public string? UnitTestingStatus { get; set; }

        public string? AcceptanceCriteria { get; set; }
    }

    public class Organization
    {
        public int? Id { get; set; }
        public string OrganizationName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber {  get; set; }
        public string? Address { get; set; }
        public string? OrganizationType { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

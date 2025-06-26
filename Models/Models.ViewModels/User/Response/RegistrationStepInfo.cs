using Models.Enums.User;


namespace Models.ViewModels.User.Response
{
    public class RegistrationStepInfo
    {
        public int StepNumber { get; set; }
        public RegistrationStatusEnum Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
    }
}
